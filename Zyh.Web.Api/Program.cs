using System.Data;
using System.Data.Common;
using Zyh.Common.Data;
using Zyh.Common.Entity.Entities;
using Zyh.Plugins.Manager;

namespace Zyh.Web.Api
{
    public class Program
    {
        private static IHost _host;
        private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #region Plugin Part

        public static PluginManager _pluginManager;
        public static FileSystemWatcher _watcher;
        public static string _pluginPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");

        #endregion

        public static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);
            var configuration = builder?.Build();

            _host = CreateHostBuilder(args).Build();

            InitPlugins();
            InitSqlSugar();
            InitDapper(configuration);

            using (var serviceScope = _host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {

                }
                catch (Exception ex)
                {
                }
            }

            _logger.Debug("���������ɹ�");

            _host.Run();
        }

        #region Plugin Func

        /// <summary>
        /// ��plugins�ļ����г�ʼ�����
        /// </summary>
        protected static void InitPlugins()
        {
            _pluginManager = new PluginManager(_pluginPath);
            var num = _pluginManager.ControlPlugin(PluginOpt.����, _pluginManager.GetPlugins);
            _logger.Debug(string.Format("һ���������{0}��,�ɹ�{1}��", _pluginManager.GetPlugins.Count, num));

            #region �����ڴ���������

            //_pluginManager = new PluginManager(string.Empty);
            //_pluginManager.PluginRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            //for (int i = 0; i < 1000; i++)
            //{
            //    // ����
            //    _pluginManager.AddPlugin("Ebos.Led.Plugin.RoadState", out Plugin newPlugin);
            //    // ����
            //    if (newPlugin != null)
            //    {
            //        _pluginManager.ControlPlugin(PluginOpt.����, new List<Plugin> { newPlugin });
            //    }
            //    // ж��
            //    _pluginManager.ControlPlugin(PluginOpt.ж��, _pluginManager.GetPlugins);
            //}

            #endregion

            #region �ļ�����

            _watcher = new FileSystemWatcher();
            _watcher.Path = _pluginPath;//���ü�ص��ļ�Ŀ¼
            _watcher.Filter = "*.dll";//���ü���ļ�������"*.txt|*.doc|*.jpg"
            _watcher.Changed += new FileSystemEventHandler(OnProcess);
            //_watcher.Created += new FileSystemEventHandler(OnProcess);
            //_watcher.Deleted += new FileSystemEventHandler(OnProcess);
            //_watcher.Renamed += new RenamedEventHandler(OnRenamed);
            //_watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            _watcher.NotifyFilter = NotifyFilters.LastAccess;
            _watcher.IncludeSubdirectories = true;//���ü��Ŀ¼�µ�������Ŀ¼
            _watcher.EnableRaisingEvents = true;//�������

            #endregion
        }

        protected static void InitSqlSugar()
        {
            #region openGauss

            //SqlSugarInstance.Type = SqlSugar.DbType.OpenGauss;
            //SqlSugarInstance.Host = "192.168.100.168";
            //SqlSugarInstance.Port = 5432;
            //SqlSugarInstance.Database = "db_led";
            //SqlSugarInstance.UserId = "test";
            //SqlSugarInstance.UserPwd = "test@123";

            #endregion

            #region dm

            //SqlSugarInstance.Type = SqlSugar.DbType.Dm;
            //SqlSugarInstance.Host = "192.168.100.198";
            //SqlSugarInstance.Port = 5236;
            //SqlSugarInstance.Database = "DMHR";
            //SqlSugarInstance.UserId = "SYSDBA";
            //SqlSugarInstance.UserPwd = "654#@!qaz";

            #endregion

            #region aliyun

            //SqlSugarInstance.Type = SqlSugar.DbType.PostgreSQL;
            //SqlSugarInstance.Host = "192.168.100.198";
            //SqlSugarInstance.Port = 5432;
            //SqlSugarInstance.Database = "db_led";
            //SqlSugarInstance.UserId = "zyh";
            //SqlSugarInstance.UserPwd = "zyh@123";

            #endregion

            #region oceanbase

            //SqlSugarInstance.Type = SqlSugar.DbType.OceanBase;
            //SqlSugarInstance.Host = "192.168.100.178";
            //SqlSugarInstance.Port = 2881;
            //SqlSugarInstance.Database = "db_led";
            //SqlSugarInstance.UserId = "root";
            //SqlSugarInstance.UserPwd = "654#@!qaz";

            #endregion
        }

        /// <summary>
        /// ��ʼ��Dapper���ݿ����ӣ��������ݿ�ʹ��
        /// </summary>
        /// <param name="configuration"></param>
        protected static void InitDapper(IConfigurationRoot configuration)
        {
            var connStr = configuration.GetSection("ConnectionStrings").GetValue<string>("DefaultConnectionString");
            DbProviderFactories.RegisterFactory("DmClientFactory", Dm.DmClientFactory.Instance); // ���Σ�ʹ����Ҫ��Ȩ�շ�
            // DbProviderFactories.RegisterFactory("Npgsql", NpgsqlFactory.Instance); // openGauss
            // DbProviderFactories.RegisterFactory("MySqlConnector", MySqlConnectorFactory.Instance); // mysql
            Environment.SetEnvironmentVariable("DefaultConnectionString.ProviderName", "DmClientFactory");
            Environment.SetEnvironmentVariable("DefaultConnectionString", connStr);

            bool btest = false;
            if (btest)
            {
                #region ����

                using (var scope = DataContextScope.GetCurrent("DefaultConnectionString").Begin(true))
                {
                    string insertSql = "INSERT INTO T_LED_DICT (NAME, CATG_ID, DICT_KEY, DICT_VALUE) VALUES (:Name, :CatgId, :DictKey, :DictValue);Select @@IDENTITY";
                    var id = scope.DataContext.ExecuteScalar<Int64>(insertSql, new { Name = "testName", CatgId = "1", DictKey = "3", DictValue = "3" });

                    scope.Commit();
                }

                #endregion

                #region ��ѯ

                var sql = "SELECT * FROM T_LED_EQUIP";

                var result1 = new List<LedEquipEntity>();
                using (var scope = DataContextScope.GetCurrent("DefaultConnectionString").Begin())
                {
                    result1 = scope.DataContext.Query<LedEquipEntity>(sql);
                }

                var result2 = new List<LedEquipEntity>();
                using (var scope = DataContextScope.GetCurrent("DefaultConnectionString").Begin())
                {
                    using var table = scope.DataContext.QueryDataTable(sql);
                    foreach (DataRow row in table.Rows)
                    {
                        LedEquipEntity entity = new LedEquipEntity
                        {
                            Id = row["id"].ToString() ?? string.Empty,
                            Name = row["name"].ToString() ?? string.Empty,
                            Type = row["type"] == DBNull.Value ? default : Convert.ToInt32(row["type"].ToString()),
                            ApplyAt = row["apply_at"] == DBNull.Value ? default : Convert.ToDateTime(row["apply_at"].ToString())
                        };

                        // �������ݿⷶΧ�Ĳ�����bool���ͣ����ص���0,1
                        string? sAlarm = row["alarm"].ToString();
                        if (int.TryParse(sAlarm, out int iAlarm))
                        {
                            entity.Alarm = Convert.ToBoolean(iAlarm);
                        }
                        else
                        {
                            entity.Alarm = Convert.ToBoolean(sAlarm);
                        }

                        result2.Add(entity);
                    }
                }

                var result3 = new List<LedEquipEntity>();
                using (var scope = DataContextScope.GetCurrent("DefaultConnectionString").Begin())
                {
                    using var cmd = scope.DataContext.DatabaseObject.GetSqlStringCommand(sql);

                    using var reader = scope.DataContext.ExecuteReader(cmd);
                    while (reader.Read())
                    {
                        // Fill����
                        var ent = new LedEquipEntity();
                        ent.Id = reader["id"].ToString() ?? string.Empty;
                        ent.Name = reader["name"].ToString() ?? string.Empty;
                        ent.Type = reader["type"] == DBNull.Value ? default : Convert.ToInt32(reader["type"].ToString());
                        ent.ApplyAt = reader["apply_at"] == DBNull.Value ? default : Convert.ToDateTime(reader["apply_at"].ToString());
                        result3.Add(ent);

                        string? sAlarm = reader["alarm"].ToString();
                        if (int.TryParse(sAlarm, out int iAlarm))
                        {
                            ent.Alarm = Convert.ToBoolean(iAlarm);
                        }
                        else
                        {
                            ent.Alarm = Convert.ToBoolean(sAlarm);
                        }
                    }
                }

                #endregion
            }
        }

        private static void OnProcess(object source, FileSystemEventArgs e)
        {
            if (e.ChangeType == WatcherChangeTypes.Changed)
            {
                OnChanged(source, e);
            }

            //if (e.ChangeType == WatcherChangeTypes.Created)
            //{
            //    OnCreated(source, e);
            //}
            //else if (e.ChangeType == WatcherChangeTypes.Changed)
            //{
            //    OnChanged(source, e);
            //}
            //else if (e.ChangeType == WatcherChangeTypes.Deleted)
            //{
            //    OnDeleted(source, e);
            //}
        }

        private static void OnCreated(object source, FileSystemEventArgs e)
        {
            //_logger.WriteLine($"�ļ��½��¼������߼� {e.ChangeType}  {e.FullPath}  {e.Name}");
        }

        private static void OnChanged(object source, FileSystemEventArgs e)
        {
            var parentDirName = Path.GetDirectoryName(e.Name);
            var dllName = Path.GetFileNameWithoutExtension(e.FullPath);
            if (parentDirName == dllName)
            {
                if (_pluginManager.IsExistsInWorkingPlugin(dllName))
                {
                    if (_pluginManager.UpdatePlugin(dllName, out Plugin newPlugin))
                    {
                        newPlugin.ControlWorkers(PluginOpt.����);
                        _logger.Debug(string.Format("���²���ɹ�:{0}", dllName));
                    }
                }
                else
                {
                    if (_pluginManager.AddPlugin(dllName, out Plugin newPlugin))
                    {
                        newPlugin.ControlWorkers(PluginOpt.����);
                        _logger.Debug(string.Format("��������ɹ�:{0}", dllName));
                    }
                }
            }
            //_logger.WriteLine($"�ļ��ı��¼������߼�{e.ChangeType}  {e.FullPath}  {e.Name}");
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            //_logger.WriteLine($"�ļ�ɾ���¼������߼�{e.ChangeType}  {e.FullPath}   {e.Name}");
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            //_logger.WriteLine($"�ļ��������¼������߼�{e.ChangeType}  {e.FullPath}  {e.Name}");
        }

        #endregion

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseKestrel(options =>
                    {
                        options.Limits.MaxRequestBodySize = null;
                    });
                }).ConfigureServices((context, services) =>
                {
                    //services.AddMetricsReportingHostedService();
                    //services.AddMetricsEndpoints(context.Configuration);
                    //services.AddMetricsTrackingMiddleware(context.Configuration);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                });

    }
}