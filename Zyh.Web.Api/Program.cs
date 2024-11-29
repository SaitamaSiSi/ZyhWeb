using System.Data;
using System.Data.Common;
using Zyh.Common.Data;
using Zyh.Common.Entity;
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

            Mysql.TempUsers.SignInClients.TryAdd(
                "vben",
                new Models.UserInfo()
                {
                    userId = "vben",
                    username = "vben",
                    realName = "VBEN"
                });
            Mysql.TempUsers.SignInClients.TryAdd(
                "admin",
                new Models.UserInfo()
                {
                    userId = "admin",
                    username = "admin",
                    realName = "ADMIN"
                });
            Mysql.TempUsers.SignInClients.TryAdd(
                "zyh",
                new Models.UserInfo()
                {
                    userId = "zyh",
                    username = "zyh",
                    realName = "ZYH"
                });

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