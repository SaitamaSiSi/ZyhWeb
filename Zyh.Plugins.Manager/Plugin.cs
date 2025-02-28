using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Zyh.Plugins.Manager
{
    public enum PluginOpt
    {
        启动 = 1,
        暂停 = 2,
        恢复 = 3,
        停止 = 4,
        卸载 = 5
    }

    public class Plugin : IDisposable
    {
        /// <summary>
        /// 插件名称
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// 插件版本
        /// </summary>
        public Version Version { get; }

        /// <summary>
        /// 生成时间
        /// </summary>
        public DateTime CompileTime { get; }

        public WeakReference WeakReference { get; private set; }

        /// <summary>
        /// 插件中符合IWorkerPlugin的实现类
        /// </summary>
        protected List<IWorkerPlugin> _workerPlugins;

        public List<IWorkerPlugin> WorkerPlugins
        {
            get
            {
                return _workerPlugins.ToList();
            }
        }

        /// <summary>
        /// 初始化插件
        /// </summary>
        /// <param name="path">插件文件路径</param>
        /// <param name="isSucess">是否初始化成功</param>
        public Plugin(string path, out bool isSucess)
        {
            isSucess = true;
            try
            {
                _workerPlugins = new List<IWorkerPlugin>();

                if (!string.IsNullOrEmpty(path))
                {
                    if (File.Exists(path))
                    {
                        Assembly assembly = LoadPlugin(path);
                        Name = assembly.GetName().Name;
                        Version = assembly.GetName().Version;
                        _workerPlugins = CreatePlugins(assembly).ToList();
                        foreach (var plugin in WorkerPlugins)
                        {
                            var iniFlag = plugin.Init();
                            Trace.WriteLine(string.Format("Create Success", plugin.Name, Name, Version, iniFlag, path));
                            isSucess = isSucess & iniFlag;
                        }
                        if (_workerPlugins.Count == 0)
                        {
                            isSucess = false;
                        }

                    }
                    else
                    {
                        Trace.WriteLine(string.Format("Create Failed", path));
                    }
                }
                else
                {
                    Trace.WriteLine("Path is empty");
                }

                if (!isSucess)
                {
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format($"Error:{0}", ex));
            }
        }

        /// <summary>
        /// 加载程序集
        /// </summary>
        /// <param name="relativePath">程序集dll文件路径</param>
        /// <returns></returns>
        protected Assembly LoadPlugin(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath) || !File.Exists(relativePath))
            {
                Trace.WriteLine(string.Format($"{0} is empty", relativePath));
                return null;
            }

            PluginLoadContext loadContext = new PluginLoadContext(relativePath);
            WeakReference = new WeakReference(loadContext, true);
            return loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(relativePath));
        }

        /// <summary>
        /// 创建插件
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <returns></returns>
        protected IEnumerable<IWorkerPlugin> CreatePlugins(Assembly assembly)
        {
            int count = 0;

            foreach (Type type in assembly.GetTypes())
            {
                if (typeof(IWorkerPlugin).IsAssignableFrom(type))
                {
                    IWorkerPlugin result = Activator.CreateInstance(type) as IWorkerPlugin;
                    if (result != null)
                    {
                        count++;
                        yield return result;
                    }
                }
            }

            if (count == 0)
            {
                string availableTypes = string.Join("\r\n", assembly.GetTypes().Select(t => t.FullName));
                Trace.WriteLine($": Can't find any type which implements IPlugin in {assembly} from {assembly.Location}.\n" +
                    $"Available types: {availableTypes}");
            }
        }

        /// <summary>
        /// 控制插件中的实现类启动，暂停，恢复，停止
        /// </summary>
        /// <param name="cmd">执行命令</param>
        /// <returns></returns>
        public int ControlWorkers(PluginOpt cmd)
        {
            var optCount = 0;

            try
            {
                foreach (var worker in WorkerPlugins)
                {
                    switch (cmd)
                    {
                        case PluginOpt.启动:
                            {
                                if (worker.Start())
                                {
                                    optCount++;
                                }
                                break;
                            }
                        case PluginOpt.暂停:
                            {
                                if (worker.Pause())
                                {
                                    optCount++;
                                }
                                break;
                            }
                        case PluginOpt.恢复:
                            {
                                if (worker.Resume())
                                {
                                    optCount++;
                                }
                                break;
                            }
                        case PluginOpt.停止:
                            {
                                if (worker.Stop())
                                {
                                    optCount++;
                                }
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("Error:{0},{1},{2}", Name, Version, ex));
                return -1;
            }

            return optCount;
        }

        /// <summary>
        /// 获取线程状态
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetWorkerStatusDesc()
        {
            Dictionary<string, string> res = new Dictionary<string, string>();
            foreach (var worker in _workerPlugins)
            {
                PluginMoreFunc.AddRange(res, worker.WorkerStatusDesc());
            }

            return res;
        }

        /// <summary>
        /// 卸载,释放资源
        /// </summary>
        public void Dispose()
        {
            if (this._workerPlugins != null)
            {
                foreach (var worker in this._workerPlugins)
                {
                    worker.Dispose();
                }
                this._workerPlugins.Clear();
                this._workerPlugins = null;
            }
            if (this.WeakReference != null)
            {
                if (this.WeakReference.IsAlive)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }
                this.WeakReference = null;
            }
        }
    }

    /// <summary>
    /// 相对适用的共用方法类库,后续看情况是否移动到Common类库
    /// </summary>
    public static class PluginMoreFunc
    {
        /// <summary>
        /// Dictionary类多个添加
        /// </summary>
        /// <typeparam name="T">Key</typeparam>
        /// <typeparam name="S">Value</typeparam>
        /// <param name="source">源字典</param>
        /// <param name="collection">添加字典</param>
        /// <param name="isUpdata">是否更新</param>
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection, bool isUpdata = false)
        {
            if (collection == null)
            {
                return;
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    if (isUpdata)
                    {
                        source.Remove(item.Key);
                        source.Add(item.Key, item.Value);
                    }
                }
            }
        }
    }
}
