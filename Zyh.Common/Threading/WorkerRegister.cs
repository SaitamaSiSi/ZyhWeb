//------------------------------------------------------------------------------
// <copyright file="WorkerRegister.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 11:05:22</date>
//------------------------------------------------------------------------------

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

