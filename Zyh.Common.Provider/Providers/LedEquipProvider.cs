//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/12 15:27:01</date>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Zyh.Common.Entity.Condition;
using Zyh.Common.Entity.Entities;
using Zyh.Common.Provider.Core;

namespace Zyh.Common.Provider.Providers
{
    public class LedEquipProvider : ProviderBase<LedEquipEntity>
    {
        protected const string TableName = "T_LED_EQUIP";

        private LedEquipEntity ConvertToEntity(DataRow row)
        {
            LedEquipEntity entity = new LedEquipEntity
            {
                Id = row["ID"].ToString() ?? string.Empty,
                Name = row["NAME"].ToString() ?? string.Empty,
                Type = row["TYPE"] == DBNull.Value ? default : Convert.ToInt32(row["type"].ToString()),
                ApplyAt = row["APPLY_AT"] == DBNull.Value ? default : Convert.ToDateTime(row["apply_at"].ToString())
            };

            string? sAlarm = row["ALARM"].ToString();
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

        private string GetWhereSql(LedEquipCondition condition, bool isLike = true)
        {
            string whereSql = string.Empty;

            if (!string.IsNullOrEmpty(condition.Id))
            {
                whereSql += " and ID = @Id ";
            }
            if (!string.IsNullOrEmpty(condition.Name))
            {
                if (isLike)
                {
                    whereSql += " and NAME like CONCAT('%', @Name, '%')";
                }
                else
                {
                    whereSql += " and NAME = @Name ";
                }
            }

            return whereSql;
        }

        private DbParameter[] GetParams(LedEquipCondition condition)
        {
            switch (DatabaseType)
            {
                case Data.ZyhDbType.Dm:
                    return GetDmParams(condition);
            }
            return new DbParameter[0];
        }

        private DbParameter[] GetDmParams(LedEquipCondition condition)
        {
            List<Dm.DmParameter> parameters = new List<Dm.DmParameter>();

            if (!string.IsNullOrEmpty(condition.Id))
            {
                parameters.Add(new Dm.DmParameter("Id", condition.Id));
            }
            if (!string.IsNullOrEmpty(condition.Name))
            {
                parameters.Add(new Dm.DmParameter("Name", condition.Name));
            }

            return parameters.ToArray();
        }

        public virtual List<LedEquipEntity> FindAll()
        {
            DataTable dt = DataContextObject.QueryDataTable($"select * from {TableName}");
            return ConvertToEntityList(dt);
        }

        public virtual List<LedEquipEntity> GetPager(LedEquipCondition condition)
        {
            string sql = $"SELECT *, ROW_NUMBER() OVER(ORDER BY NOW())AS RowIndex from {TableName} where 1=1 ";
            string whereSql = GetWhereSql(condition);
            DbParameter[] parameters = GetParams(condition);

            int startIndex = (condition.PageIndex - 1) * condition.PageSize + 1;
            int endIndex = condition.PageIndex * condition.PageSize;
            string pagerSql = CheckSqlChar($"select * from ({sql + whereSql}) AS subquery where RowIndex between {startIndex} and {endIndex}");

            DataTable dt = DataContextObject.QueryDataTable(pagerSql, parameters);
            return ConvertToEntityList(dt);
        }

        public virtual LedEquipEntity Get(LedEquipCondition condition)
        {
            string selectSql = $"SELECT * FROM {TableName} WHERE 1=1 ";
            string whereSql = GetWhereSql(condition, false);
            DbParameter[] parameters = GetParams(condition);

            string sql = CheckSqlChar(selectSql + whereSql + " Limit 1");

            DataTable dt = DataContextObject.QueryDataTable(sql, parameters);
            return ConvertToEntityList(dt).FirstOrDefault() ?? new LedEquipEntity();
        }

        public virtual int Insert(LedEquipEntity entity)
        {
            string sql = CheckSqlChar($"INSERT INTO {TableName} VALUES (@Id, @Name, @Type, @Alarm, @ApplyAt)");
            return DataContextObject.ExecuteScalar<int>(sql, new
            {
                Id = entity.Id,
                Name = entity.Name,
                Type = entity.Type ?? 0,
                Alarm = entity.Alarm ?? false,
                ApplyAt = entity.ApplyAt ?? DateTime.Now
            });
        }

        public virtual int Update(LedEquipEntity entity)
        {
            string sql = CheckSqlChar($"UPDATE {TableName} SET NAME=@Name, TYPE=@Type, ALARM=@Alarm WHERE ID=@Id ");
            return DataContextObject.Execute(sql, entity);
        }

        public virtual int Delete(LedEquipCondition condition)
        {
            string sql = CheckSqlChar($"DELETE FROM {TableName} WHERE ID IN @Ids)");
            return DataContextObject.Execute(sql, condition);
        }
    }
}
