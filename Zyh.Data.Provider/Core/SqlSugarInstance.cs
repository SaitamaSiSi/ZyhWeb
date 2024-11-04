//------------------------------------------------------------------------------
// <copyright file="SqlSugarInstance.cs" company="CQ ULIT Co., Ltd.">
//    Copyright (c) 2024, Chongqing Youliang Science & Technology Co., Ltd. All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/28 17:28:17</date>
//------------------------------------------------------------------------------

using SqlSugar;

namespace Zyh.Data.Provider.Core
{
    public class SqlSugarInstance
    {
        public static string Host { get; set; } = "127.0.0.1";
        public static int Port { get; set; } = 5432;
        public static string Database { get; set; } = "postgres";
        public static string UserId { get; set; } = string.Empty;
        public static string UserPwd { get; set; } = string.Empty;

        public static DbType Type = DbType.PostgreSQL;
        static ThreadLocal<SqlSugarScope> Local = new ThreadLocal<SqlSugarScope>();

        protected static ConnectionConfig CreateConnectionConfig(string connectString)
        {
            ConnectionConfig config = new ConnectionConfig();
            config.ConnectionString = connectString;
            config.DbType = Type;
            config.IsAutoCloseConnection = true;

            if (Type == DbType.Dm)
            {
                config.MoreSettings = new ConnMoreSettings()
                {
                    IsAutoToUpper = false // 禁用自动转成大写表 5.1.3.41-preview04
                };
            }
            else if (Type == DbType.OceanBase)
            {
                config.MoreSettings = new ConnMoreSettings()
                {
                    DisableNvarchar = true //个别特殊的数据库需要禁用Nvarchar
                };
            }

            return config;
        }

        public static SqlSugarScope GetCurrent(string connectString)
        {
            SqlSugarScope? ctx;
            ctx = Local.Value;

            if (ctx == null)
            {
                ctx = new SqlSugarScope(CreateConnectionConfig(connectString));
                Local.Value = ctx;
            }

            return ctx;
        }

        public static string GetConnectString()
        {
            if (string.IsNullOrEmpty(Host) || Port < 0 || Port > 65535 ||
                string.IsNullOrEmpty(Database) || string.IsNullOrEmpty(UserId)
                || string.IsNullOrEmpty(UserPwd))
            {
                return string.Empty;
            }

            switch (Type)
            {
                case DbType.OpenGauss:
                    return $"HOST={Host};PORT={Port};DATABASE={Database};USER ID={UserId};PASSWORD={UserPwd};No Reset On Close=true";
                case DbType.Dm:
                    return $"Server={Host};PORT={Port};DATABASE={Database};USER ID={UserId};PWD={UserPwd};SCHEMA={Database}";
                case DbType.PostgreSQL:
                    //常见错误 Unsupported command 特殊网络会出现 //加上禁用连接池
                    return $"HOST={Host};PORT={Port};DATABASE={Database};USER ID={UserId};PASSWORD={UserPwd}";
                case DbType.OceanBase:
                    return $"Server={Host};PORT={Port};Database={Database};Uid={UserId};Pwd={UserPwd}";
                default:
                    return string.Empty;
            }
        }
    }
}
