//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 11:03:01</date>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using Zyh.Common.Threading;
using Zyh.Plugin.Template.Workers;
using Zyh.Plugins;
using System.Linq;

namespace Zyh.Plugin.Template
{
    public class ViolationInfoPlugin : IWorkerPlugin
    {
        //protected string _pluginName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name.ToString();
        public string Name
        {
            get
            {
                return "模板插件";
            }
        }

        protected readonly WorkerManager _workerManager = new WorkerManager();

        public bool Init()
        {
            //var iniPath = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), (_pluginName + ".ini"));

            var doPlayWorker = new DoPlayWorker();
            _workerManager.Register(doPlayWorker.Name, doPlayWorker);

            return true;
        }

        public bool Start()
        {
            return _workerManager.StartAll();
        }

        public bool Pause()
        {
            return _workerManager.SuspendAll();
        }

        public bool Resume()
        {
            return _workerManager.ResumeAll();
        }

        public bool Stop()
        {
            return _workerManager.StopAll();
        }

        public Dictionary<string, string> WorkerStatusDesc()
        {
            var res = new Dictionary<string, string>();

            foreach (var worker in _workerManager)
            {
                if (worker.Value != null)
                {
                    res.TryAdd(worker.Key, worker.Value.StatusDesc());
                }
            }

            return res;
        }

        public void Dispose()
        {
            this.Stop();
            if (this._workerManager != null)
            {
                foreach (var worker in this._workerManager.ToList())
                {
                    this._workerManager.UnRegister(worker.Key);
                }
                this._workerManager.Clear();
            }
        }
    }
}
