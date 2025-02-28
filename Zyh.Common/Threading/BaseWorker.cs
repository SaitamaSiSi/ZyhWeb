using System;
using System.Diagnostics;
using System.Threading;

namespace Zyh.Common.Threading
{
    public abstract class BaseWorker : IWorker
    {
        protected Int32 _stopRequested = -1;
        protected Int32 _pauseRequested = -1;
        private Int64 _status;

        protected virtual bool StopRequested
        {
            get
            {
                bool result = Interlocked.CompareExchange(ref _stopRequested, 1, 1) == 1;

                return result;
            }
        }

        protected virtual bool PauseRequested
        {
            get
            {
                bool result = Interlocked.CompareExchange(ref _pauseRequested, 1, 1) == 1;

                return result;
            }
        }

        public abstract string Name { get; }

        public virtual WorkerStatus Status
        {
            get
            {
                var value = Interlocked.Read(ref _status);
                return (WorkerStatus)value;
            }
            protected set
            {
                Int64 input = (Int64)value;
                Interlocked.Exchange(ref _status, input);
            }
        }

        WorkerStatus IWorker.Status { get => this.Status; }

        public abstract void Start();

        public abstract void Stop();

        public virtual void Pause()
        {
            Interlocked.Exchange(ref _pauseRequested, 1);
            Status = WorkerStatus.Paused;

            Trace.WriteLine(string.Format("{0} 已暂停", this.Name));
        }

        protected virtual void DoResume()
        {
            Interlocked.Exchange(ref _pauseRequested, 0);
            Status = WorkerStatus.Running;
        }

        public virtual void Resume()
        {
            DoResume();

            Trace.WriteLine(string.Format("{0} 已恢复", this.Name));
        }

        public virtual void Dispose()
        {
            Status = WorkerStatus.Disposed;
        }
    }
}
