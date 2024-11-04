//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 15:35:02</date>
//------------------------------------------------------------------------------

using SqlSugar;
using Zyh.Data.Entity.Condition;
using Zyh.Data.Entity.OpenGauss;
using Zyh.Data.Provider.Core;
using Zyh.Data.Provider.OpenGauss;
using Zyh.Data.Service.Core;

namespace Zyh.Data.Service.OpenGauss
{
    public class LedEquipService : ServiceBase<LedEquipEntity>
    {
        protected readonly LedEquipProvider Provider;
        public LedEquipService(string connectString)
        {
            ConnectString = connectString;
            Provider = new LedEquipProvider(ConnectString);
        }

        public void Test(bool b)
        {
            using (SqlSugarScope scope = SqlSugarInstance.GetCurrent(ConnectString))
            {
                try
                {
                    scope.BeginTran();
                    Provider.Insert(new LedEquipEntity() { Name = "test1" });
                    Provider.Insert(new LedEquipEntity() { Name = "test2" });
                    if (b)
                    {
                        throw new Exception("异常");
                    }
                    scope.CommitTran();
                }
                catch (Exception)
                {
                    scope.RollbackTran();
                }
            }
        }

        public List<LedEquipEntity> FindAll()
        {
            using (SqlSugarScope scope = SqlSugarInstance.GetCurrent(ConnectString))
            {
                return Provider.FindAll();
            }
        }

        public List<LedEquipEntity> GetPager(LedEquipCondition condition)
        {
            using (SqlSugarScope scope = SqlSugarInstance.GetCurrent(ConnectString))
            {
                return Provider.GetPager(condition);
            }
        }

        public LedEquipEntity Get(LedEquipCondition condition)
        {
            using (SqlSugarScope scope = SqlSugarInstance.GetCurrent(ConnectString))
            {
                return Provider.Get(condition);
            }
        }

        public bool Insert(LedEquipEntity entity)
        {
            using (SqlSugarScope scope = SqlSugarInstance.GetCurrent(ConnectString))
            {
                return Provider.Insert(entity) > 0;
            }
        }

        public bool Update(LedEquipEntity entity)
        {
            using (SqlSugarScope scope = SqlSugarInstance.GetCurrent(ConnectString))
            {
                return Provider.Update(entity) > 0;
            }
        }

        public bool Delete(LedEquipCondition condition)
        {
            using (SqlSugarScope scope = SqlSugarInstance.GetCurrent(ConnectString))
            {
                return Provider.Delete(condition) > 0;
            }
        }
    }
}
