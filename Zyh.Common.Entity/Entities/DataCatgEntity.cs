//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/13 14:38:27</date>
//------------------------------------------------------------------------------

using System;

namespace Zyh.Common.Entity
{
    public class DataCatgEntity : ICloneable, IEntity
    {
        /// <summary>
        /// get or set 分类
        /// </summary>
        public virtual String Id { get; set; }

        /// <summary>
        /// get or set 描述
        /// </summary>
        public virtual String Remark { get; set; }

        public virtual DataCatgEntity CloneFrom(DataCatgEntity thatObj)
        {
            if (thatObj == null)
            {
                throw new ArgumentNullException("thatObj");
            }

            this.Id = thatObj.Id;
            this.Remark = thatObj.Remark;

            return this;
        }

        public virtual DataCatgEntity CloneTo(DataCatgEntity thatObj)
        {
            if (thatObj == null)
            {
                throw new ArgumentNullException("thatObj");
            }

            thatObj.Id = this.Id;
            thatObj.Remark = this.Remark;

            return thatObj;
        }

        public virtual DataCatgEntity Clone()
        {
            var thatObj = new DataCatgEntity();

            return this.CloneTo(thatObj);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}

