//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/18 16:04:42</date>
//------------------------------------------------------------------------------


namespace Zyh.Common.Service.Base
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using Zyh.Common.Data;
    using Zyh.Common.Entity;
    using Zyh.Common.Provider;
    using Zyh.Common.Service;

    public abstract partial class DataDictSqlServiceBase : SqlServiceBase<DataDictEntity>
    {
        private readonly String _connectionName;

        protected virtual String ConnectionName
        {
            get { return _connectionName; }
        }

        protected readonly DataDictSqlProvider Provider;

        public DataDictSqlServiceBase(String connectionName)
        {
            _connectionName = connectionName;
            Provider = new DataDictSqlProvider(connectionName);
        }

        public virtual Boolean Exists(Int32 id)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return Provider.Exists(id);
            }
        }

        public virtual DataDictEntity Get(Int32 id)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return Provider.Get(id);
            }
        }

        public virtual List<DataDictEntity> FindAll(string whereClause)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectionName).Begin())
            {
                return Provider.FindAll(whereClause);
            }
        }

        public virtual Boolean Add(DataDictEntity ent)
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

        public virtual Boolean Add(IEnumerable<DataDictEntity> list)
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

        public virtual Boolean Update(DataDictEntity ent)
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

        public virtual Boolean Delete(Int32 id)
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
