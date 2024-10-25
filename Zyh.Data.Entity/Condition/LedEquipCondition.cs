//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/25 10:01:31</date>
//------------------------------------------------------------------------------

namespace Zyh.Data.Entity.Condition
{
    public class LedEquipCondition : PagerCondition
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public List<string> Ids { get; set; } = new List<string>();
    }
}
