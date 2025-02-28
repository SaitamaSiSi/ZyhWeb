using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Zyh.Common.Threading
{
    public class QueueWorker<T> : ThreadWorker, IWorker
    {
        protected static ConcurrentQueue<T> Queue = new ConcurrentQueue<T>();

        public static void Enqueue(T item)
        {
            if (Queue.Count > 100000)
            {
                throw new ArgumentException("队列数量超过100000");
            }

            Queue.Enqueue(item);
        }

        protected virtual IList<T> TryDequeue(Int32 capacity)
        {
            List<T> result = new List<T>(capacity);
            Int32 index = 0;

            while (!Queue.IsEmpty)
            {
                if (index >= capacity)
                {
                    break;
                }

                T item;
                if (!Queue.TryDequeue(out item))
                {
                    break;
                }

                result.Add(item);
                index++;
            }

            return result;
        }
    }
}
