//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/12 15:23:25</date>
//------------------------------------------------------------------------------

using System;
using Zyh.Common.Entity.Core;

namespace Zyh.Common.Entity.Entities
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
