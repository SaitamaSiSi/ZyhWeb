//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/13 15:38:17</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Zyh.Common.Service.Base;

namespace Zyh.Common.Service.Services
{
    public partial class DataDictSqlService : DataDictSqlServiceBase
    {
        const String DefaultConnectionString = "DefaultConnectionString";

        #region Constructor

        public DataDictSqlService() : this(DefaultConnectionString)
        {
        }

        public DataDictSqlService(String connectionName) : base(connectionName)
        {
        }

        #endregion
    }
}
