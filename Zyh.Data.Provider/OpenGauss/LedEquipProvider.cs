//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 14:57:06</date>
//------------------------------------------------------------------------------

using SqlSugar;
using System.Data;
using System.Security.Claims;
using Zyh.Data.Entity.Condition;
using Zyh.Data.Entity.OpenGauss;
using Zyh.Data.Provider.Core;

namespace Zyh.Data.Provider.OpenGauss
{
    public class LedEquipProvider : ProviderBase<LedEquipEntity>
    {
        protected const string TableName = "T_LED_EQUIP";

        public LedEquipProvider(string connectString)
        {
            ConnectString = connectString;
        }

        private LedEquipEntity ConvertToEntity(DataRow row)
        {
            LedEquipEntity entity = new LedEquipEntity
            {
                Id = row["id"].ToString() ?? string.Empty,
                Name = row["name"].ToString() ?? string.Empty,
                Type = row["type"] == DBNull.Value ? default : Convert.ToInt32(row["type"].ToString()),
                ApplyAt = row["apply_at"] == DBNull.Value ? default : Convert.ToDateTime(row["apply_at"].ToString())
            };

            string? sAlarm = row["alarm"].ToString();
            if (int.TryParse(sAlarm, out int iAlarm))
            {
                entity.Alarm = Convert.ToBoolean(iAlarm);
            }
            else
            {
                entity.Alarm = Convert.ToBoolean(sAlarm);
            }

            return entity;
        }

        private List<LedEquipEntity> ConvertToEntityList(DataTable dataTable)
        {
            List<LedEquipEntity> entityList = new List<LedEquipEntity>();

            foreach (DataRow row in dataTable.Rows)
            {
                entityList.Add(ConvertToEntity(row));
            }

            return entityList;
        }

        private Dictionary<string, object> ConvertFromEntity(LedEquipEntity entity, bool isUpdate = false)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            dict.Add("id", entity.Id);

            if (!isUpdate || (isUpdate && !string.IsNullOrEmpty(entity.Name)))
            {
                dict.Add("name", entity.Name);
            }
            if (!isUpdate || (isUpdate && entity.Type != null))
            {
                dict.Add("type", entity.Type ?? 0);
            }
            if (!isUpdate || (isUpdate && entity.Alarm != null))
            {
                dict.Add("alarm", entity.Alarm ?? false);
            }
            if (!isUpdate || (isUpdate && entity.ApplyAt != null))
            {
                dict.Add("apply_at", entity.ApplyAt ?? DateTime.Now);
            }

            return dict;
        }

        private string GetSql(LedEquipCondition condition)
        {
            string whereSql = string.Empty;

            if (!string.IsNullOrEmpty(condition.Id))
            {
                whereSql += " and " + (IsLikeMysql ? "id" : "\"id\"") + " = @Id ";
            }
            if (!string.IsNullOrEmpty(condition.Name))
            {
                whereSql += " and " + (IsLikeMysql ? "name" : "\"name\"") + " like @Name ";
            }

            return whereSql;
        }

        private Dictionary<string, object> GetParams(LedEquipCondition condition)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(condition.Id))
            {
                dict.Add("Id", condition.Id);
            }
            if (!string.IsNullOrEmpty(condition.Name))
            {
                dict.Add("Name", $"%{condition.Name}%");
            }

            return dict;
        }

        public virtual List<LedEquipEntity> FindAll()
        {
            DataTable dt = SqlSugarScopeObject.Ado.GetDataTable($"select * from {TableName}");
            return ConvertToEntityList(dt);
        }

        public virtual List<LedEquipEntity> GetPager(LedEquipCondition condition)
        {
            string sql = $"select *, ROW_NUMBER() OVER(ORDER BY now())AS RowIndex from {TableName} where 1=1 ";
            string whereSql = GetSql(condition);
            Dictionary<string, object> parameters = GetParams(condition);

            int startIndex = (condition.PageIndex - 1) * condition.PageSize + 1;
            int endIndex = condition.PageIndex * condition.PageSize;
            string pagerSql = $"select * from ({sql + whereSql}) AS subquery where RowIndex between {startIndex} and {endIndex}";

            DataTable dt = SqlSugarScopeObject.Ado.GetDataTable(pagerSql, parameters);
            return ConvertToEntityList(dt);
        }

        public virtual LedEquipEntity Get(LedEquipCondition condition)
        {
            string sql = $"select * from {TableName} where 1=1 ";
            string whereSql = GetSql(condition);
            Dictionary<string, object> parameters = GetParams(condition);

            DataTable dt = SqlSugarScopeObject.Ado.GetDataTable(sql + whereSql + " Limit 1", parameters);
            return ConvertToEntityList(dt).FirstOrDefault() ?? new LedEquipEntity();
        }

        public virtual int Insert(LedEquipEntity entity)
        {
            if (string.IsNullOrEmpty(entity.Id))
            {
                long snowId = SnowFlakeSingle.Instance.NextId();
                entity.Id = snowId.ToString();
                return SqlSugarScopeObject.Insertable(ConvertFromEntity(entity)).AS(TableName).ExecuteCommand();
            }
            else
            {
                return SqlSugarScopeObject.Insertable(ConvertFromEntity(entity)).AS(TableName).ExecuteCommand();
            }
        }

        public virtual int Update(LedEquipEntity entity)
        {
            return SqlSugarScopeObject.Updateable(ConvertFromEntity(entity, true)).AS(TableName).WhereColumns("id").ExecuteCommand();
        }

        public virtual int Delete(LedEquipCondition condition)
        {
            string whereSql = (IsLikeMysql ? "id" : "\"id\"") + " in (@Id) ";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Id", condition.Ids);
            return SqlSugarScopeObject.Deleteable<object>().AS(TableName).Where(whereSql, parameters).ExecuteCommand();
        }
    }
}
