using ProtoBuf;

namespace Zyh.Client.Test
{
    /// <summary>
    /// 该类基于protobuf-net，属于第三方实现，无需proto文件，适用于纯 .NET、快速开发、代码优先
    /// </summary>
    [ProtoContract]
    public class Person
    {
        [ProtoMember(1)]  // 字段编号必须唯一
        public string Name { get; set; } = string.Empty;

        [ProtoMember(2)]
        public int Id { get; set; }

        [ProtoMember(3)]
        public List<string> Emails { get; set; } = new List<string>();
    }
}
