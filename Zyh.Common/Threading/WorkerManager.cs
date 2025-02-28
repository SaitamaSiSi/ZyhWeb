using System;
using System.Diagnostics;
using System.Linq;

namespace Zyh.Common.Threading
{
    public class WorkerManager : WorkerRegister
    {
        public bool StartAll()
        {
            try
            {
                var workerNames = this.Keys.ToArray();
                foreach (var workerName in workerNames)
                {
                    if (this.TryGetValue(workerName, out var worker))
                    {
                        worker.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("线程管理启动所有线程异常:{0}", ex.ToString()));
                return false;
            }

            return true;
        }

        public bool StopAll()
        {
            try
            {
                Trace.WriteLine("线程管理停止所有线程");
                var workerNames = this.Keys.ToArray();
                foreach (var workerName in workerNames)
                {
                    if (this.TryGetValue(workerName, out var worker))
                    {
                        worker.Stop();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("线程管理停止所有线程异常:{0}", ex.ToString()));
                return false;
            }

            return true;
        }

        public bool SuspendAll()
        {
            try
            {
                var workerNames = this.Keys.ToArray();
                foreach (var workerName in workerNames)
                {
                    if (this.TryGetValue(workerName, out var worker))
                    {
                        worker.Pause();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("线程管理暂停所有线程异常:{0}", ex.ToString()));
                return false;
            }

            return true;
        }

        public bool ResumeAll()
        {
            try
            {
                var workerNames = this.Keys.ToArray();
                foreach (var workerName in workerNames)
                {
                    if (this.TryGetValue(workerName, out var worker))
                    {
                        worker.Resume();
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(string.Format("线程管理恢复所有线程异常:{0}", ex.ToString()));
                return false;
            }

            return true;
        }
    }
}
