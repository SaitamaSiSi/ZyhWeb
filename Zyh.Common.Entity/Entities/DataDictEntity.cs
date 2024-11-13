//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/13 14:38:46</date>
//------------------------------------------------------------------------------

using System;
using Zyh.Common.Entity.Core;

namespace Zyh.Common.Entity.Entities
{
    public class DataDictEntity : ICloneable, IEntity
    {
        /// <summary>
        /// get or set 编号
        /// </summary>
        public virtual Int32 Id { get; set; }

        /// <summary>
        /// get or set 分类
        /// </summary>
        public virtual String CatgId { get; set; }

        /// <summary>
        /// get or set 字典健
        /// </summary>
        public virtual String DictKey { get; set; }

        /// <summary>
        /// get or set 字典值
        /// </summary>
        public virtual String DictValue { get; set; }

        /// <summary>
        /// get or set 应用系统编号
        /// </summary>
        public virtual String SysId { get; set; }

        /// <summary>
        /// get or set 是否启用
        /// </summary>
        public virtual Boolean Enabled { get; set; }

        /// <summary>
        /// get or set 标签
        /// </summary>
        public virtual String Tag { get; set; }

        /// <summary>
        /// get or set 父级编号
        /// </summary>
        public virtual String ParentKey { get; set; }

        /// <summary>
        /// get or set 图标
        /// </summary>
        public virtual String Icon { get; set; }

        /// <summary>
        /// get or set 字典层级
        /// </summary>
        public virtual Int32 DictLevel { get; set; }

        /// <summary>
        /// get or set 排序编号
        /// </summary>
        public virtual Int32 SortIndex { get; set; }

        /// <summary>
        /// get or set 备注
        /// </summary>
        public virtual String Remark { get; set; }

        /// <summary>
        /// get or set 修改者
        /// </summary>
        public virtual String ModifiedBy { get; set; }

        /// <summary>
        /// get or set 修改时间
        /// </summary>
        public virtual DateTime ModifiedAt { get; set; }

        public virtual DataDictEntity CloneFrom(DataDictEntity thatObj)
        {
            if (thatObj == null)
            {
                throw new ArgumentNullException("thatObj");
            }

            this.Id = thatObj.Id;
            this.CatgId = thatObj.CatgId;
            this.DictKey = thatObj.DictKey;
            this.DictValue = thatObj.DictValue;
            this.SysId = thatObj.SysId;
            this.Enabled = thatObj.Enabled;
            this.Tag = thatObj.Tag;
            this.ParentKey = thatObj.ParentKey;
            this.Icon = thatObj.Icon;
            this.DictLevel = thatObj.DictLevel;
            this.SortIndex = thatObj.SortIndex;
            this.Remark = thatObj.Remark;
            this.ModifiedBy = thatObj.ModifiedBy;
            this.ModifiedAt = thatObj.ModifiedAt;

            return this;
        }

        public virtual DataDictEntity CloneTo(DataDictEntity thatObj)
        {
            if (thatObj == null)
            {
                throw new ArgumentNullException("thatObj");
            }

            thatObj.Id = this.Id;
            thatObj.CatgId = this.CatgId;
            thatObj.DictKey = this.DictKey;
            thatObj.DictValue = this.DictValue;
            thatObj.SysId = this.SysId;
            thatObj.Enabled = this.Enabled;
            thatObj.Tag = this.Tag;
            thatObj.ParentKey = this.ParentKey;
            thatObj.Icon = this.Icon;
            thatObj.DictLevel = this.DictLevel;
            thatObj.SortIndex = this.SortIndex;
            thatObj.Remark = this.Remark;
            thatObj.ModifiedBy = this.ModifiedBy;
            thatObj.ModifiedAt = this.ModifiedAt;

            return thatObj;
        }

        public virtual DataDictEntity Clone()
        {
            var thatObj = new DataDictEntity();

            return this.CloneTo(thatObj);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }
    }
}
