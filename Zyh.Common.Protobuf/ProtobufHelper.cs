using Google.Protobuf;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace Zyh.Common.Protobuf
{
    /// <summary>
    /// 该类是基于Google.Protobuf，属于官方实现，需要proto文件，使用于多语言、严格协议规范、gRPC 集成	
    /// </summary>
    public class ProtobufHelper
    {
        public static byte[] Serialize<T>(T obj) where T : IMessage<T>
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            try
            {
                using (var ms = new MemoryStream())
                {
                    obj.WriteTo(ms);
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                throw new SerializationException("Protobuf序列化失败", ex);
            }
        }

        public static T Deserialize<T>(byte[] data) where T : IMessage<T>, new()
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            try
            {
                var parser = new MessageParser<T>(() => new T());
                using (var ms = new MemoryStream(data))
                {
                    return parser.ParseFrom(ms);
                }
            }
            catch (InvalidProtocolBufferException ex)
            {
                throw new SerializationException("Protobuf反序列化失败：数据格式错误", ex);
            }
            catch (Exception ex)
            {
                throw new SerializationException("Protobuf反序列化失败", ex);
            }
        }
    }
}

