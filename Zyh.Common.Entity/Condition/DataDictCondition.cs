//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/13 15:02:13</date>
//------------------------------------------------------------------------------

namespace Zyh.Common.Entity.Condition
{
    public class DataDictCondition : PagerCondition
    {
        /// <summary>
        /// ID
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// 字典分类
        /// </summary>
        public string CatgId { get; set; }

        /// <summary>
        /// 字典键
        /// </summary>
        public string DictKey { get; set; }

        /// <summary>
        /// 字典值
        /// </summary>
        public string DictValue { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// 启用/禁用
        /// </summary>
        public bool? Enabled { get; set; }
    }
}
