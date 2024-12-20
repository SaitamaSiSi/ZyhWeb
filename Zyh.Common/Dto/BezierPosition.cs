//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/12/19 17:31:38</date>
//------------------------------------------------------------------------------

namespace Zyh.Common.Dto
{
    public class BezierPosition
    {
        /// <summary>
        /// X坐标
        /// </summary>
        public float X { get; set; }

        /// <summary>
        /// Y坐标
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        /// 交通情况, 16进制色彩值
        /// </summary>
        public string Status { get; set; } = "#00FF00";

        /// <summary>
        /// 是否控制点
        /// </summary>
        public bool IsControl { get; set; }
    }
}
