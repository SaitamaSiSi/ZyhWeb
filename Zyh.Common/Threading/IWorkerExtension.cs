//------------------------------------------------------------------------------
// <copyright file="IWorkerExtension.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 11:15:37</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Text;

namespace Zyh.Common.Threading
{
    public static class IWorkerExtension
    {
        public static string StatusDesc(this IWorker worker)
        {
            if (worker == null)
            {
                return string.Empty;
            }

            switch (worker.Status)
            {
                case WorkerStatus.Created:
                    return "创建";
                case WorkerStatus.Running:
                    return "运行";
                case WorkerStatus.Paused:
                    return "暂停";
                case WorkerStatus.Stopped:
                    return "停止";
                case WorkerStatus.Disposed:
                    return "释放";
                default:
                    break;
            }

            return string.Empty;
        }
    }
}
