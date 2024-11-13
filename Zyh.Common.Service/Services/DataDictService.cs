//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/13 15:38:17</date>
//------------------------------------------------------------------------------

using System;
using Zyh.Common.Data;
using Zyh.Common.Entity.Entities;
using Zyh.Common.Provider.Providers;
using Zyh.Common.Service.Core;

namespace Zyh.Common.Service.Services
{
    public class DataDictService : ServiceBase<DataDictEntity>
    {
        protected readonly DataDictProvider Provider = new DataDictProvider();

        public void Test()
        {
            using (var scope = DataContextScope.GetCurrent(ConnectString).Begin(true))
            {
                try
                {
                    Provider.Test();
                    scope.Commit();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
