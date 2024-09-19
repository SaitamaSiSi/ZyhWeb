//------------------------------------------------------------------------------
// <copyright file="IWorkerPlugin.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 10:37:10</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace Zyh.Plugins
{
    public interface IWorkerPlugin : IDisposable
    {
        string Name { get; }

        bool Init();

        bool Start();

        bool Pause();

        bool Resume();

        bool Stop();

        Dictionary<string, string> WorkerStatusDesc();
    }
}
