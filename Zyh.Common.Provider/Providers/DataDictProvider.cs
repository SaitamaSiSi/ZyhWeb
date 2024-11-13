//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/13 15:36:36</date>
//------------------------------------------------------------------------------

using System;
using Zyh.Common.Entity.Entities;
using Zyh.Common.Provider.Core;

namespace Zyh.Common.Provider.Providers
{
    public class DataDictProvider : ProviderBase<DataDictEntity>
    {
        protected const string TableName = "T_SYS_DICT_DATA";

        public void Test()
        {
            string sql = CheckSqlChar($"INSERT INTO {TableName} (CATG_ID, DICT_KEY, DICT_VALUE, ENABLED, DICT_LEVEL, SORT_INDEX, MODIFIED_AT) VALUES (@CatgId, @DictKey, @DictValue, @Enabled, @DictLevel, @SortIndex, @ModifiedAt)") + GetAutoIncrement();
            int id = DataContextObject.ExecuteScalar<int>(sql, new
            {
                CatgId = "DisplayMode",
                DictKey = "测试自增",
                DictValue = "996",
                Enabled = false,
                DictLevel = 1,
                SortIndex = 24,
                ModifiedAt = DateTime.Now
            });
        }
    }
}
