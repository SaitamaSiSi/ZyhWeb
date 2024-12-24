//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/12/24 9:27:28</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Concurrent;

namespace Zyh.Common.IO
{
    public class BufferPool
    {
        private readonly ConcurrentDictionary<Int64, ConcurrentBag<byte[]>> _objects;

        public BufferPool()
        {
            _objects = new ConcurrentDictionary<Int64, ConcurrentBag<byte[]>>();
        }

        public byte[] Get(Int64 size)
        {
            var bag = _objects.GetOrAdd(size, new ConcurrentBag<byte[]>());

            var result = bag.TryTake(out var buffer) ? buffer : new Byte[size];

            return result;
        }

        public void Return(byte[] item)
        {
            if (item == null)
            {
                return;
            }

            var newValue = new ConcurrentBag<byte[]>();
            newValue.Add(item);

            _objects.AddOrUpdate(item.Length, newValue, (m, n) =>
            {
                n.Add(item);
                return n;
            });
        }

        public void Clear()
        {
            _objects.Clear();
        }

        public void Invoke(Action<ConcurrentDictionary<Int64, ConcurrentBag<byte[]>>> action)
        {
            if (action == null)
            {
                return;
            }

            action(_objects);
        }

        //public override string ToString()
        //{
        //    //var result = string.Join(", ", _objects.OrderBy(m => m.Key).Select(m =>
        //    //    ByteSizeLib.ByteSize.FromBytes(m.Key).ToString() 
        //    //    + "=" 
        //    //    + ByteSizeLib.ByteSize.FromBytes(m.Key * m.Value.Count).ToString()));
        //    var result = ByteSize.FromBytes(_objects.Sum(m => m.Key * m.Value.Count)).ToString();
        //    return result;
        //}
    }
}
