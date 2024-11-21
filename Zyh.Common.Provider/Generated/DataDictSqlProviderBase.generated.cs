//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/15 11:02:09</date>
//------------------------------------------------------------------------------

namespace Zyh.Common.Provider.Base
{
    using Dm;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Zyh.Common.Entity;

    /// <summary>
	/// Data access base class for DataDictEntity
    /// </summary>
    public abstract partial class DataDictSqlProviderBase : SqlProviderBase<DataDictEntity>
    {
        #region SQL

        protected const string TableName = "T_SYS_DICT_DATA";

        protected const string Exists_DataDict_Sql = @"SELECT COUNT(*) FROM T_SYS_DICT_DATA 
WHERE ID=:Id";

        protected const string Get_DataDict_Sql = @"SELECT * FROM T_SYS_DICT_DATA 
WHERE ID=:Id";

        protected const string Find_DataDict_Sql = @"SELECT * FROM T_SYS_DICT_DATA WHERE 1=1 ";

        protected const string Insert_DataDict_Sql = @"
INSERT INTO T_SYS_DICT_DATA
VALUES
(:Id, :CatgId, :DictKey, :DictValue, :SysId, :Enabled, :Tag, :ParentKey, :Icon, :DictLevel, :SortIndex, :Remark, :ModifiedBy, :ModifiedAt)";

        protected const string Update_DataDict_Sql = @"
UPDATE T_SYS_DICT_DATA
SET ID=:Id, CATG_ID=:CatgId, DICT_KEY=:DictKey, DICT_VALUE=:DictValue, SYS_ID=:SysId, ENABLED=:Enabled, TAG=:Tag, PARENT_KEY=:ParentKey, ICON=:Icon, DICT_LEVEL=:DictLevel, SORT_INDEX=:SortIndex, REMARK=:Remark, MODIFIED_BY=:ModifiedBy, MODIFIED_AT=:ModifiedAt
WHERE ID=:Id";

        protected const string Delete_DataDict_Sql = @"
DELETE FROM T_SYS_DICT_DATA
WHERE ID=:Id";

        #endregion

        #region Constructor

        public DataDictSqlProviderBase() : base()
        {
        }

        public DataDictSqlProviderBase(string connectionName) : base(connectionName)
        {
        }

        #endregion

        #region CRUD

        public virtual Boolean Exists(Int32 id)
        {
            using var cmd = DatabaseObject.GetSqlStringCommand(Exists_DataDict_Sql);
            cmd.Parameters.Add(new DmParameter("Id", DmDbType.VarChar) { Value = id, Direction = ParameterDirection.Input });
            var result = DataContextObject.ExecuteScalar(cmd);

            return Convert.ToInt32(result) > 0;
        }

        public virtual DataDictEntity Get(Int32 id)
        {
            DataDictEntity result = null;
            using var cmd = DatabaseObject.GetSqlStringCommand(Get_DataDict_Sql);

            cmd.Parameters.Add(new DmParameter("Id", DmDbType.VarChar) { Value = id, Direction = ParameterDirection.Input });
            using (var reader = DataContextObject.ExecuteReader(cmd))
            {
                if (reader.Read())
                {
                    result = Fill(reader);
                }
            }

            return result;
        }

        public virtual List<DataDictEntity> FindAll(string whereClause)
        {
            var result = new List<DataDictEntity>();
            var sql = Find_DataDict_Sql + whereClause;
            using var cmd = DatabaseObject.GetSqlStringCommand(sql);

            using (var reader = DataContextObject.ExecuteReader(cmd))
            {
                while (reader.Read())
                {
                    var ent = Fill(reader);
                    result.Add(ent);
                }
            }

            return result;
        }

        public virtual Int32 Add(DataDictEntity entity)
        {
            var isAutoGetNewId = entity.Id == default(Int32);
            using var cmd = DatabaseObject.GetSqlStringCommand(Insert_DataDict_Sql) as DmCommand;

            var pkParam = new DmParameter("Id", DmDbType.Int32) { Value = entity.Id, Direction = ParameterDirection.Input };
            cmd.Parameters.Add(pkParam);
            var nonKeyParams = BuildParametersForNonKey(entity);
            cmd.Parameters.AddRange(nonKeyParams);

            var execResult = DataContextObject.ExecuteNonQuery(cmd);
            if (isAutoGetNewId)
            {
                int lastInsertId = DataContextObject.Execute("Select @@IDENTITY;");
                if (lastInsertId > 0)
                {
                    entity.Id = lastInsertId;
                }
            }

            return execResult;
        }

        public virtual Int32 Add(IEnumerable<DataDictEntity> list)
        {
            if (list == null || !list.Any())
            {
                return 0;
            }

            const string InsertIntoClause = "INSERT INTO T_SYS_DICT_DATA VALUES ";
            var valueClauses = new List<string>(list.Count());
            var index = 0;
            foreach (var entity in list)
            {
                var clause = $"(:id_{index}, :catg_id_{index}, :dict_key_{index}, :dict_value_{index}, :sys_id_{index}, :enabled_{index}, :tag_{index}, :parent_key_{index}, :icon_{index}, :dict_level_{index}, :sort_index_{index}, :remark_{index}, :modified_by_{index}, :modified_at_{index}";
                valueClauses.Add(clause);
                index++;
            }
            var sql = InsertIntoClause + string.Join(",", valueClauses) + ";";
            using var cmd = DatabaseObject.GetSqlStringCommand(sql) as DmCommand;
            var @parameters = BuildParameters(list);
            cmd.Parameters.AddRange(@parameters);

            return DataContextObject.ExecuteNonQuery(cmd);
        }

        public virtual Int32 Update(DataDictEntity entity)
        {
            var @parameters = BuildParameters(entity);
            using var cmd = DatabaseObject.GetSqlStringCommand(Update_DataDict_Sql) as DmCommand;
            cmd.Parameters.AddRange(@parameters);

            return DataContextObject.ExecuteNonQuery(cmd);
        }

        public virtual Int32 Delete(Int32 id)
        {
            using var cmd = DatabaseObject.GetSqlStringCommand(Delete_DataDict_Sql);

            cmd.Parameters.Add(new DmParameter("Id", DmDbType.Int32) { Value = id, Direction = ParameterDirection.Input });

            return DataContextObject.ExecuteNonQuery(cmd);
        }

        #region Build Parameters

        public virtual DbParameter[] BuildParameters(DataDictEntity entity)
        {
            if (entity == null)
            {
                return null;
            }

            var @paramList = new DmParameter[14];
            @paramList[0] = new DmParameter("Id", DmDbType.Int32) { Value = entity.Id, Direction = ParameterDirection.Input };
            @paramList[1] = new DmParameter("CatgId", DmDbType.VarChar) { Value = entity.CatgId, Direction = ParameterDirection.Input };
            @paramList[2] = new DmParameter("DictKey", DmDbType.VarChar) { Value = entity.DictKey, Direction = ParameterDirection.Input };
            @paramList[3] = new DmParameter("DictValue", DmDbType.VarChar) { Value = entity.DictValue, Direction = ParameterDirection.Input };
            @paramList[4] = new DmParameter("SysId", DmDbType.VarChar) { Value = entity.SysId, Direction = ParameterDirection.Input };
            @paramList[5] = new DmParameter("Enabled", DmDbType.Byte) { Value = entity.Enabled, Direction = ParameterDirection.Input };
            @paramList[6] = new DmParameter("Tag", DmDbType.VarChar) { Value = entity.Tag, Direction = ParameterDirection.Input };
            @paramList[7] = new DmParameter("ParentKey", DmDbType.VarChar) { Value = entity.ParentKey, Direction = ParameterDirection.Input };
            @paramList[8] = new DmParameter("Icon", DmDbType.VarChar) { Value = entity.Icon, Direction = ParameterDirection.Input };
            @paramList[9] = new DmParameter("DictLevel", DmDbType.Int32) { Value = entity.DictLevel, Direction = ParameterDirection.Input };
            @paramList[10] = new DmParameter("SortIndex", DmDbType.Int32) { Value = entity.SortIndex, Direction = ParameterDirection.Input };
            @paramList[11] = new DmParameter("Remark", DmDbType.VarChar) { Value = entity.Remark, Direction = ParameterDirection.Input };
            @paramList[12] = new DmParameter("ModifiedBy", DmDbType.VarChar) { Value = entity.ModifiedBy, Direction = ParameterDirection.Input };
            @paramList[13] = new DmParameter("ModifiedAt", DmDbType.DateTime) { Value = entity.ModifiedAt, Direction = ParameterDirection.Input };

            return @paramList;
        }

        public virtual DbParameter[] BuildParameters(IEnumerable<DataDictEntity> list)
        {
            if (list == null || !list.Any())
            {
                return null;
            }

            var @paramList = new List<DmParameter>(list.Count() * 14);
            var index = 0;
            foreach (var entity in list)
            {
                @paramList.Add(new DmParameter($"id_{index}", DmDbType.Int32) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"catg_id_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"dict_key_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"dict_value_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"sys_id_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"enabled_{index}", DmDbType.Byte) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"tag_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"parent_key_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"icon_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"dict_level_{index}", DmDbType.Int32) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"sort_index_{index}", DmDbType.Int32) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"remark_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"modified_by_{index}", DmDbType.VarChar) { Value = entity.Id, Direction = ParameterDirection.Input });
                @paramList.Add(new DmParameter($"modified_at_{index}", DmDbType.DateTime) { Value = entity.Id, Direction = ParameterDirection.Input });
            }

            return @paramList.ToArray();
        }

        public virtual DbParameter[] BuildParametersForNonKey(DataDictEntity entity)
        {

            var @paramList = new DmParameter[13];
            @paramList[0] = new DmParameter("CatgId", DmDbType.VarChar) { Value = entity.CatgId, Direction = ParameterDirection.Input };
            @paramList[1] = new DmParameter("DictKey", DmDbType.VarChar) { Value = entity.DictKey, Direction = ParameterDirection.Input };
            @paramList[2] = new DmParameter("DictValue", DmDbType.VarChar) { Value = entity.DictValue, Direction = ParameterDirection.Input };
            @paramList[3] = new DmParameter("SysId", DmDbType.VarChar) { Value = entity.SysId, Direction = ParameterDirection.Input };
            @paramList[4] = new DmParameter("Enabled", DmDbType.Byte) { Value = entity.Enabled, Direction = ParameterDirection.Input };
            @paramList[5] = new DmParameter("Tag", DmDbType.VarChar) { Value = entity.Tag, Direction = ParameterDirection.Input };
            @paramList[6] = new DmParameter("ParentKey", DmDbType.VarChar) { Value = entity.ParentKey, Direction = ParameterDirection.Input };
            @paramList[7] = new DmParameter("Icon", DmDbType.VarChar) { Value = entity.Icon, Direction = ParameterDirection.Input };
            @paramList[8] = new DmParameter("DictLevel", DmDbType.Int32) { Value = entity.DictLevel, Direction = ParameterDirection.Input };
            @paramList[9] = new DmParameter("SortIndex", DmDbType.Int32) { Value = entity.SortIndex, Direction = ParameterDirection.Input };
            @paramList[10] = new DmParameter("Remark", DmDbType.VarChar) { Value = entity.Remark, Direction = ParameterDirection.Input };
            @paramList[11] = new DmParameter("ModifiedBy", DmDbType.VarChar) { Value = entity.ModifiedBy, Direction = ParameterDirection.Input };
            @paramList[12] = new DmParameter("ModifiedAt", DmDbType.DateTime) { Value = entity.ModifiedAt, Direction = ParameterDirection.Input };

            return @paramList;
        }

        #endregion

        #endregion

        #region Fill Data

        public static DataDictEntity Fill(IDataReader reader)
        {
            var ent = new DataDictEntity();

            ent.Id = reader["ID"] == DBNull.Value ? default(Int32) : reader.GetInt32(reader.GetOrdinal("ID"));
            ent.CatgId = reader["CATG_ID"] == DBNull.Value ? default(String) : reader.GetString(reader.GetOrdinal("CATG_ID"));
            ent.DictKey = reader["DICT_KEY"] == DBNull.Value ? default(String) : reader.GetString(reader.GetOrdinal("DICT_KEY"));
            ent.DictValue = reader["DICT_VALUE"] == DBNull.Value ? default(String) : reader.GetString(reader.GetOrdinal("DICT_VALUE"));
            ent.SysId = reader["SYS_ID"] == DBNull.Value ? default(String) : reader.GetString(reader.GetOrdinal("SYS_ID"));
            ent.Enabled = reader["ENABLED"] == DBNull.Value ? default(Boolean) : Convert.ToBoolean(reader.GetByte(reader.GetOrdinal("ENABLED")));
            ent.Tag = reader["TAG"] == DBNull.Value ? null : reader.GetString(reader.GetOrdinal("TAG"));
            ent.ParentKey = reader["PARENT_KEY"] == DBNull.Value ? null : reader.GetString(reader.GetOrdinal("PARENT_KEY"));
            ent.Icon = reader["ICON"] == DBNull.Value ? null : reader.GetString(reader.GetOrdinal("ICON"));
            ent.DictLevel = reader["DICT_LEVEL"] == DBNull.Value ? default(Int32) : reader.GetInt32(reader.GetOrdinal("DICT_LEVEL"));
            ent.SortIndex = reader["SORT_INDEX"] == DBNull.Value ? default(Int32) : reader.GetInt32(reader.GetOrdinal("SORT_INDEX"));
            ent.Remark = reader["REMARK"] == DBNull.Value ? null : reader.GetString(reader.GetOrdinal("REMARK"));
            ent.ModifiedBy = reader["MODIFIED_BY"] == DBNull.Value ? null : reader.GetString(reader.GetOrdinal("MODIFIED_BY"));
            ent.ModifiedAt = reader["MODIFIED_AT"] == DBNull.Value ? default(DateTime) : reader.GetDateTime(reader.GetOrdinal("MODIFIED_AT"));

            return ent;
        }

        #endregion
    }
}
