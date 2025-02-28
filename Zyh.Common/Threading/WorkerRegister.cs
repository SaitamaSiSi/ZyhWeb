using System.Collections.Generic;

namespace Zyh.Common.Threading
{
    public class WorkerRegister : Dictionary<string, IWorker>
    {
        public void Register(string workerName, IWorker worker)
        {
            if (this.ContainsKey(workerName))
            {
                return;
            }

            this[workerName] = worker;
        }

        public bool UnRegister(string workerName)
        {
            return this.Remove(workerName);
        }

        public IWorker GetWorker(string workerName)
        {
            if (this.ContainsKey(workerName))
            {
                return this[workerName];
            }

            return null;
        }
    }
}

