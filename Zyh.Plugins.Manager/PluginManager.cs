//------------------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 11:23:24</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Zyh.Plugins.Manager
{
    public class PluginManager
    {
        /// <summary>
        /// 插件集合
        /// </summary>
        protected List<Plugin> Plugins;

        /// <summary>
        /// 插件根路径 plugins文件夹
        /// </summary>
        public string PluginRootPath { get; set; }

        public List<Plugin> GetPlugins
        {
            get
            {
                return Plugins.ToList();
            }
        }

        /// <summary>
        /// 初始化插件管理工具
        /// </summary>
        /// <param name="path">插件根路径</param>
        public PluginManager(string path)
        {
            try
            {
                PluginRootPath = path;
                Plugins = new List<Plugin>();

                if (!string.IsNullOrEmpty(path))
                {
                    if (!Directory.Exists(path))
                    {
                        //创建文件夹
                        Trace.WriteLine(string.Format($"创建文件夹:{0}", path));
                        Directory.CreateDirectory(path);
                    }

                    DirectoryInfo mydir = new DirectoryInfo(path);
                    var dirs = mydir.GetDirectories();
                    foreach (var dir in dirs)
                    {
                        var dllPath = Path.Combine(dir.FullName, dir.Name + ".dll");
                        if (File.Exists(dllPath))
                        {
                            var plugin = new Plugin(dllPath, out bool isSucess);
                            if (plugin == null || !isSucess)
                            {
                                Trace.WriteLine(string.Format($"插件创建失败:{0}", dllPath));
                                continue;
                            }
                            Plugins.Add(plugin);
                        }
                        else
                        {
                            Trace.WriteLine(string.Format($"插件路径不存在:{0}", dllPath));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format($"创建插件发生异常:{0}", ex));
            }
        }

        /// <summary>
        /// 添加插件
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <returns></returns>
        public bool AddPlugin(string pluginName, out Plugin newPlugin)
        {
            newPlugin = null;
            try
            {
                if (string.IsNullOrEmpty(PluginRootPath))
                {
                    Trace.WriteLine($"AddPlugin: 插件根路径为空");
                    return false;
                }
                var oldPlugins = Plugins.Where(m => string.Equals(pluginName, m.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                if (oldPlugins.Count > 0)
                {
                    Trace.WriteLine(string.Format($"插件已存在:{0}", pluginName));
                    return false;
                }

                if (!Directory.Exists(PluginRootPath))
                {
                    //创建文件夹
                    Trace.WriteLine(string.Format($"创建插件文件夹:{0}", PluginRootPath));
                    Directory.CreateDirectory(PluginRootPath);
                }

                var path = Path.Combine(PluginRootPath, pluginName);
                var dllPath = Path.Combine(path, pluginName + ".dll");
                if (Directory.Exists(path) && File.Exists(dllPath))
                {
                    newPlugin = new Plugin(dllPath, out bool isSucess);
                    if (newPlugin == null || !isSucess)
                    {
                        Trace.WriteLine(string.Format($"插件新增失败:{0}", dllPath));
                        return false;
                    }
                    Plugins.Add(newPlugin);
                }
                else
                {
                    Trace.WriteLine(string.Format($"插件路径不存在:{0},{1}", path, dllPath));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format($"新增插件发生异常:{0}", ex));
                return false;
            }

            return true;
        }

        public bool IsExistsInWorkingPlugin(string pluginName)
        {
            return GetPlugins.Where(m => string.Equals(pluginName, m.Name, StringComparison.OrdinalIgnoreCase)).ToList().Count > 0;
        }

        /// <summary>
        /// 更新插件
        /// </summary>
        /// <param name="pluginName">插件名称</param>
        /// <returns></returns>
        public bool UpdatePlugin(string pluginName, out Plugin newPlugin)
        {
            newPlugin = null;
            try
            {
                if (string.IsNullOrEmpty(PluginRootPath))
                {
                    Trace.WriteLine($"UpdatePlugin: 插件根路径为空");
                    return false;
                }
                var oldPlugins = Plugins.Where(m => string.Equals(pluginName, m.Name, StringComparison.OrdinalIgnoreCase)).ToList();
                if (oldPlugins.Count == 0)
                {
                    Trace.WriteLine(string.Format($"原插件不存在:{0}", pluginName));
                    return false;
                }
                var count = RemotePlugin(oldPlugins);

                if (count == oldPlugins.Count)
                {
                    AddPlugin(pluginName, out newPlugin);
                }
                else
                {
                    Trace.WriteLine(string.Format($"原插件卸载失败:{0}", pluginName));
                    return false;
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format($"更新插件发生异常{0}", ex));
                return false;
            }

            return true;
        }

        /// <summary>
        /// 删除插件
        /// </summary>
        /// <param name="remotePlugins">插件集合</param>
        /// <returns></returns>
        public int RemotePlugin(List<Plugin> remotePlugins)
        {
            var remoteCount = 0;

            try
            {
                foreach (var remotePlugin in remotePlugins)
                {
                    if (remotePlugin == null)
                    {
                        Trace.WriteLine("插件不存在");
                        return 0;
                    }

                    if (Plugins.Contains(remotePlugin))
                    {
                        remotePlugin.ControlWorkers(PluginOpt.卸载);
                        Plugins.Remove(remotePlugin);
                        remotePlugin.Dispose();
                        remoteCount++;
                    }
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format($"插件移除异常:{0}", ex));
                return -1;
            }

            return remoteCount;
        }

        /// <summary>
        /// 控制插件启动，暂停，恢复，停止
        /// </summary>
        /// <param name="cmd">执行命令</param>
        /// <param name="plugins">插件集合</param>
        /// <returns></returns>
        public int ControlPlugin(PluginOpt cmd, List<Plugin> plugins)
        {
            var optCount = 0;

            try
            {
                foreach (var plugin in plugins)
                {
                    if (Plugins.Contains(plugin))
                    {
                        if (cmd == PluginOpt.卸载)
                        {
                            if (Plugins.Remove(plugin))
                            {
                                plugin.Dispose();
                                optCount++;
                            }
                        }
                        else
                        {
                            if (plugin.ControlWorkers(cmd) > 0)
                            {
                                optCount++;
                            }
                        }
                    }
                }
                GC.Collect();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format($"控制插件异常:{0}", ex));
                return -1;
            }

            return optCount;
        }

        /// <summary>
        /// 获取目标插件的线程状态
        /// </summary>
        /// <param name="pluginName"></param>
        /// <returns></returns>
        public Dictionary<string, string> GetWorkerStatusDesc(string pluginName)
        {

            Dictionary<string, string> res = new Dictionary<string, string>();

            var plugins = Plugins.Where(m => string.Equals(pluginName, m.Name, StringComparison.OrdinalIgnoreCase)).ToList();
            foreach (var plugin in plugins)
            {
                PluginMoreFunc.AddRange(res, plugin.GetWorkerStatusDesc());
            }

            return res;
        }
    }
}
