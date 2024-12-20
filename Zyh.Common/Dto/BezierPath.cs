//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/12/20 9:34:44</date>
//------------------------------------------------------------------------------

using System.Collections.Generic;

namespace Zyh.Common.Dto
{
    public class BezierPath
    {
        /// <summary>
        /// 曲线为每段颜色各不同的曲线，辅助线为颜色一致的补充填色线
        /// </summary>
        public BezierType Shape { get; set; } = BezierType.曲线;

        /// <summary>
        /// 分段数量
        /// </summary>
        public int? SplitNum { get; set; }

        /// <summary>
        /// 线宽度
        /// </summary>
        public int LineWidth { get; set; } = 10;

        /// <summary>
        /// 起点节点
        /// </summary>
        public BezierPosition Source { get; set; } = new BezierPosition();

        /// <summary>
        /// 终点节点
        /// </summary>
        public BezierPosition Target { get; set; } = new BezierPosition();

        /// <summary>
        /// 中间坐标，从起点到终点的顺序放置
        /// </summary>
        public List<BezierPosition> Vertices { get; set; } = new List<BezierPosition>();
    }
}
