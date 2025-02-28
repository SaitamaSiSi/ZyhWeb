using System;

namespace Zyh.Common.Threading
{
    public interface IWorker : IDisposable
    {
        string Name { get; }

        WorkerStatus Status { get; }

        void Start();

        void Pause();

        void Resume();

        void Stop();
    }
}
