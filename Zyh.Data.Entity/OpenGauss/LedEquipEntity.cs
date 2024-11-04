//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 14:57:48</date>
//------------------------------------------------------------------------------

using Zyh.Data.Entity.Core;

namespace Zyh.Data.Entity.OpenGauss
{
    public class LedEquipEntity : IEntity
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int? Type { get; set; }
        public bool? Alarm { get; set; }
        public DateTime? ApplyAt { get; set; }
    }
}
