//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 15:35:02</date>
//------------------------------------------------------------------------------

using Zyh.Data.Entity.Condition;
using Zyh.Data.Entity.OpenGauss;
using Zyh.Data.Provider.OpenGauss;
using Zyh.Data.Service.Core;

namespace Zyh.Data.Service.OpenGauss
{
    public class LedEquipService : ServiceBase<LedEquipEntity>
    {
        protected readonly LedEquipProvider Provider;
        public LedEquipService(string connectString)
        {
            Provider = new LedEquipProvider(connectString);
        }

        public List<LedEquipEntity> FindAll()
        {
            return Provider.FindAll();
        }

        public List<LedEquipEntity> GetPager(LedEquipCondition condition)
        {
            return Provider.GetPager(condition);
        }

        public LedEquipEntity Get(LedEquipCondition condition)
        {
            return Provider.Get(condition);
        }

        public bool Insert(LedEquipEntity entity)
        {
            return Provider.Insert(entity) > 0;
        }

        public bool Update(LedEquipEntity entity)
        {
            return Provider.Update(entity) > 0;
        }

        public bool Delete(LedEquipCondition condition)
        {
            return Provider.Delete(condition) > 0;
        }
    }
}
