//------------------------------------------------------------------------------
// <copyright file="WorkerStatus.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 11:06:25</date>
//------------------------------------------------------------------------------

using System.ComponentModel;

namespace Zyh.Common.Threading
{
    public enum WorkerStatus
    {
        [Description("Created")]
        Created = 0,

        [Description("Running")]
        Running,

        [Description("Paused")]
        Paused,

        [Description("Stopped")]
        Stopped,

        [Description("Disposed")]
        Disposed
    }
}
