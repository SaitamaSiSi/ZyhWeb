//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Zyh.Common.Service.Base
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Zyh.Common.Entity;
    using Zyh.Common.Provider;
    using Zyh.Common.Service;
    using Zyh.Common.Data;
    
    
    public abstract partial class LedEquipSqlServiceBase : SqlServiceBase<LedEquipEntity>
    {
        
        private String _connectionName;
        
        protected LedEquipSqlProvider Provider;
        
        public LedEquipSqlServiceBase(String connectionName)
        {

            _connectionName = connectionName;
            Provider = new LedEquipSqlProvider(connectionName);
        }
        
        protected virtual String ConnectionName
        {
            get
            {
                return this._connectionName;
            }
        }
        
        public virtual Boolean Exists(String id)
        {

            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return Provider.Exists(id);
            }
        }
        
        public virtual LedEquipEntity Get(String id)
        {

            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return Provider.Get(id);
            }
        }
        
        public virtual List<LedEquipEntity> FindAll(String whereClause)
        {

            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return Provider.FindAll(whereClause);
            }
        }
        
        public virtual List<LedEquipEntity> GetPager(Int32 pageIndex, Int32 pageSize, String whereClause)
        {

            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return Provider.GetPager(pageIndex, pageSize, whereClause);
            }
        }
        
        public virtual Boolean Add(LedEquipEntity ent)
        {

            if (ent == null)
            {
                throw new ArgumentNullException("ent");
            }
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin(true))
            {
                var result = Provider.Add(ent) > 0;
                scope.Commit();
                return result;
            }
        }
        
        public virtual Boolean Add(IEnumerable<LedEquipEntity> list)
        {

            if (list == null)
            {
                throw new ArgumentNullException("list");
            }
            var count = list.Count();
            if (count == 0)
            {
                return false;
            }
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin(true))
            {
                var result = Provider.Add(list) == count;
                scope.Commit();
                return result;
            }
        }
        
        public virtual Boolean Update(LedEquipEntity ent)
        {

            if (ent == null)
            {
                throw new ArgumentNullException("ent");
            }
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin(true))
            {
                var result = Provider.Update(ent) > 0;
                scope.Commit();
                return result;
            }
        }
        
        public virtual Boolean Delete(String id)
        {

            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin(true))
            {
                var result = Provider.Delete(id) > 0;
                scope.Commit();
                return result;
            }
        }
    }
}
