//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 14:57:06</date>
//------------------------------------------------------------------------------

using SqlSugar;
using System.Data;
using Zyh.Data.Entity.Condition;
using Zyh.Data.Entity.OpenGauss;
using Zyh.Data.Provider.Core;

namespace Zyh.Data.Provider.OpenGauss
{
    public class LedEquipProvider : ProviderBase<LedEquipEntity>
    {
        protected const string TableName = "t_led_equip";
        protected readonly SqlSugarClient Provider;

        public LedEquipProvider(string connectString)
        {
            ConnectString = connectString;
            Provider = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectString,
                DbType = SqlSugar.DbType.PostgreSQL,
                IsAutoCloseConnection = true//自动释放
            });
        }

        private LedEquipEntity ConvertToEntity(DataRow row)
        {
            LedEquipEntity entity = new LedEquipEntity
            {
                Id = row["id"].ToString() ?? string.Empty,
                Name = row["name"].ToString() ?? string.Empty,
                Type = row["type"] == DBNull.Value ? default : Convert.ToInt32(row["type"].ToString()),
                Alarm = row["alarm"] == DBNull.Value ? default : Convert.ToBoolean(row["alarm"].ToString()),
                ApplyAt = row["apply_at"] == DBNull.Value ? default : Convert.ToDateTime(row["apply_at"].ToString())
            };
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
                whereSql += " and id = @Id ";
            }
            if (!string.IsNullOrEmpty(condition.Name))
            {
                whereSql += " and name like @Name ";
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
            DataTable dt = Provider.Ado.GetDataTable($"select * from {TableName}");
            return ConvertToEntityList(dt);
        }

        public virtual List<LedEquipEntity> GetPager(LedEquipCondition condition)
        {
            string sql = $"select *, ROW_NUMBER() OVER(ORDER BY now())AS RowIndex from {TableName} where 1=1 ";
            string whereSql = GetSql(condition);
            Dictionary<string, object> parameters = GetParams(condition);

            int startIndex = (condition.PageIndex - 1) * condition.PageSize + 1;
            int endIndex = condition.PageIndex * condition.PageSize;
            string pagerSql = $"select * from ({sql + whereSql}) where RowIndex between {startIndex} and {endIndex}";

            DataTable dt = Provider.Ado.GetDataTable(pagerSql, parameters);
            return ConvertToEntityList(dt);
        }

        public virtual LedEquipEntity Get(LedEquipCondition condition)
        {
            string sql = $"select * from {TableName} where 1=1 ";
            string whereSql = GetSql(condition);
            Dictionary<string, object> parameters = GetParams(condition);

            DataTable dt = Provider.Ado.GetDataTable(sql + whereSql + " Limit 1", parameters);
            return ConvertToEntityList(dt).FirstOrDefault() ?? new LedEquipEntity();
        }

        public virtual int Insert(LedEquipEntity entity)
        {
            return Provider.Insertable(ConvertFromEntity(entity)).AS(TableName).ExecuteCommand();
        }

        public virtual int Update(LedEquipEntity entity)
        {
            return Provider.Updateable(ConvertFromEntity(entity, true)).AS(TableName).WhereColumns("id").ExecuteCommand();
        }

        public virtual int Delete(LedEquipCondition condition)
        {
            string whereSql = "id in (@Id) ";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            parameters.Add("Id", condition.Ids);
            return Provider.Deleteable<object>().AS(TableName).Where(whereSql, parameters).ExecuteCommand();
        }

        //db.Insertable(insertObj).ExecuteCommand(); //都是参数化实现 
        //db.Insertable(insertObj).ExecuteReturnIdentity();
        //返回雪花ID 看文档3.1具体用法（在最底部）
        //long id = db.Insertable(实体).ExecuteReturnSnowflakeId();
    }
}
