//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/12 15:20:40</date>
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Zyh.Common.Entity.Condition
{
    public class LedEquipCondition : PagerCondition
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Ids { get; set; } = new List<string>();
    }
}
