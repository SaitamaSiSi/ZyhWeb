//------------------------------------------------------------------------------
// <copyright file="IWorker.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 11:05:54</date>
//------------------------------------------------------------------------------

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
