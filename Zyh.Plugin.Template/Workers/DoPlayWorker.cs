//------------------------------------------------------------------------------
// <copyright file="DoPlayWorker.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 11:08:41</date>
//------------------------------------------------------------------------------

using System;
using System.Threading;
using Zyh.Common.Threading;

namespace Zyh.Plugin.Template.Workers
{
    public class DoPlayWorker : QueueWorker<string>
    {

        public DoPlayWorker() { }

        protected override void DoWork()
        {
            try
            {
                // TODO
                Console.WriteLine("TemplatePlugin线程运行中...");
            }
            catch (Exception ex)
            {
                Thread.Sleep(1000);
            }
            //Thread.Sleep(5);
            Thread.Sleep(1000 * 60);
        }
    }
}
