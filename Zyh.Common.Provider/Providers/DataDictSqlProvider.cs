//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/13 15:36:36</date>
//------------------------------------------------------------------------------

using Zyh.Common.Provider.Base;

namespace Zyh.Common.Provider
{
    /// <summary>
    /// Data access for DataDictEntity
    /// </summary>
    public partial class DataDictSqlProvider : DataDictSqlProviderBase
    {
        #region Constructor

        public DataDictSqlProvider() : base()
        {
        }

        public DataDictSqlProvider(string connectionName) : base(connectionName)
        {
        }

        #endregion
    }
}
