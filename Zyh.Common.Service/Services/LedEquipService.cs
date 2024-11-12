//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/12 15:31:53</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Zyh.Common.Data;
using Zyh.Common.Entity.Condition;
using Zyh.Common.Entity.Entities;
using Zyh.Common.Provider.Providers;
using Zyh.Common.Service.Core;

namespace Zyh.Common.Service.Services
{
    public class LedEquipService : ServiceBase<LedEquipEntity>
    {
        protected readonly LedEquipProvider Provider = new LedEquipProvider();

        public void Test(bool b)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectString).Begin(true))
            {
                try
                {
                    Provider.Insert(new LedEquipEntity() { Name = "test1" });
                    Provider.Insert(new LedEquipEntity() { Name = "test2" });
                    if (b)
                    {
                        throw new Exception("异常");
                    }
                    scope.Commit();
                }
                catch (Exception)
                {
                }
            }
        }

        public List<LedEquipEntity> FindAll()
        {
            using (var scope = DataContextScope.GetCurrent(ConnectString).Begin())
            {
                return Provider.FindAll();
            }
        }

        public List<LedEquipEntity> GetPager(LedEquipCondition condition)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectString).Begin())
            {
                return Provider.GetPager(condition);
            }
        }

        public LedEquipEntity Get(LedEquipCondition condition)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectString).Begin())
            {
                return Provider.Get(condition);
            }
        }

        public bool Insert(LedEquipEntity entity)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectString).Begin())
            {
                return Provider.Insert(entity) > 0;
            }
        }

        public bool Update(LedEquipEntity entity)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectString).Begin())
            {
                return Provider.Update(entity) > 0;
            }
        }

        public bool Delete(LedEquipCondition condition)
        {
            using (var scope = DataContextScope.GetCurrent(ConnectString).Begin())
            {
                return Provider.Delete(condition) > 0;
            }
        }
    }
}
