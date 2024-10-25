//------------------------------------------------------------------------------
// <copyright file="ThreadWorker.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 11:09:50</date>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Threading;

namespace Zyh.Common.Threading
{
    public class ThreadWorker : BaseWorker, IWorker
    {
        private readonly Action _methodToRunInLoop;
        private Thread _thread;
        protected string _workerName;

        public override string Name
        {
            get { return this.ThreadName; }
        }

        protected virtual string ThreadName
        {
            get
            {
                return _workerName + "." + GetThreadId().ToString();
            }
        }

        protected virtual bool IsBackground
        {
            get
            {
                return false;
            }
        }

        public ThreadWorker()
            : this(null)
        {
            _workerName = this.GetType().Name;
            _methodToRunInLoop = DoWork;
        }

        public ThreadWorker(Action methodToRunInLoop)
        {
            _workerName = this.GetType().Name;
            _methodToRunInLoop = methodToRunInLoop;
            this.InitThread();
        }

        protected virtual Thread InitThread()
        {
            this._thread = new Thread(new ThreadStart(this.Loop));
            this._thread.Name = this.ThreadName;
            this._thread.IsBackground = this.IsBackground;
            Status = WorkerStatus.Created;

            return this._thread;
        }

        public override void Start()
        {
            Interlocked.Exchange(ref _stopRequested, 0);

            if (!this._thread.IsAlive)
            {
                if (Status == WorkerStatus.Disposed || Status == WorkerStatus.Stopped)
                {
                    InitThread().Start();
                }
                else
                {
                    if (Status == WorkerStatus.Created)
                    {
                        this._thread.Start();
                    }
                }
            }

            DoResume();
            Status = WorkerStatus.Running;

            Trace.WriteLine(string.Format("{0} 已启动", this.Name));
        }

        public override void Stop()
        {
            Interlocked.Exchange(ref _stopRequested, 1);
            Status = WorkerStatus.Stopped;

            Trace.WriteLine(string.Format("{0} 已停止", this.Name));
        }

        protected void Loop()
        {
            while (!StopRequested)
            {
                try
                {
                    if (PauseRequested)
                    {
                        Thread.Sleep(10);
                        continue;
                    }

                    _methodToRunInLoop();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(string.Format("{0}中发生异常:{1}", this.Name, ex));
                }
                finally
                {
                    Thread.Sleep(0);
                }
            }
        }

        protected virtual void DoWork()
        {
            Trace.Write("//TODO Override");
        }

        protected Int32 GetThreadId()
        {
            return _thread.ManagedThreadId;
        }
    }
}
