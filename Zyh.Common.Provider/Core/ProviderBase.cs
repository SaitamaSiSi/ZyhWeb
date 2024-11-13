//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/12 15:26:01</date>
//------------------------------------------------------------------------------

using Zyh.Common.Data;
using Zyh.Common.Entity.Core;

namespace Zyh.Common.Provider.Core
{
    public class ProviderBase<T> where T : IEntity
    {
        protected string ConnectString = "DefaultConnectionString";

        protected ZyhDbType DatabaseType
        {
            get
            {
                return DataContextObject.DatabaseType;
            }
        }

        protected bool IsLikeOracle
        {
            get
            {
                if (DatabaseType == ZyhDbType.Dm)
                {
                    return true;
                }
                return false;
            }
        }

        protected string GetSqlChar
        {
            get
            {
                if (IsLikeOracle)
                {
                    return ":";
                }
                return "@";
            }
        }

        protected Database DatabaseObject
        {
            get
            {
                return DataContextObject.DatabaseObject;
            }
        }

        protected DataContext DataContextObject
        {
            get
            {
                return DataContextScope.GetCurrent(ConnectString).DataContext;
            }
        }

        protected string CheckSqlChar(string sql)
        {
            if (IsLikeOracle)
            {
                return sql.Replace('@', ':');
            }
            else
            {
                return sql.Replace(':', '@');
            }
        }

        protected string GetAutoIncrement()
        {
            return DatabaseType == Data.ZyhDbType.Dm ? "; Select @@IDENTITY; " : "";
        }
    }
}
