//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 14:48:58</date>
//------------------------------------------------------------------------------

using Zyh.Data.Entity.Core;

namespace Zyh.Data.Provider.Core
{
    public class ProviderBase<T> where T : IEntity
    {
        protected string ConnectString = "";

        protected SqlSugar.SqlSugarScope SqlSugarScopeObject
        {
            get { return SqlSugarInstance.GetCurrent(ConnectString); }
        }

        protected SqlSugar.DbType SqlSugarDbType
        {
            get { return SqlSugarInstance.Type; }
        }

        protected bool IsLikeMysql
        {
            get
            {
                if (SqlSugarInstance.Type == SqlSugar.DbType.OceanBase)
                {
                    return true;
                }
                return false;
            }
        }

        protected string SqlSugarDatabase
        {
            get { return SqlSugarInstance.Database; }
        }
    }
}
