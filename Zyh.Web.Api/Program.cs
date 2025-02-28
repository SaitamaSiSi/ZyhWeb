using System.Data.Common;
using Zyh.Common.AI;
using Zyh.Common.Security;
using Zyh.Plugins.Manager;
using Zyh.Web.Api.Worker;
using static SkiaSharp.HarfBuzz.SKShaper;

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
            #region 测试分类识别

            string classifyOnnx = "F:\\TestProgram\\Test\\AIStudio\\已上传到群中的模型文件\\分类\\zyh图像分类_飞桨ResNet50_0.0.2\\231013_Auto0.3_只有交通标牌_缩放左上角\\model_new.onnx";
            var classifier = new OnnxImageClassifier(classifyOnnx, new string[] { "r", "y", "g", "o", "n" });
            var dict = classifier.ClassifyImage("F:\\TestProgram\\Test\\AIStudio\\分类模型暂存\\十二生肖分类数据模型\\signs\\test\\r.jpg");
            Console.WriteLine($"分类结果: {dict.FirstOrDefault().Key}-{dict.FirstOrDefault().Value}");

            #endregion

            #region 测试目标识别

            //string detectOnnx = "F:\\TestProgram\\Test\\AIStudio\\目标模型暂存\\train_720_1280混合训练_增加全局注意力(GAM)机制_替换c2f_增加自定义数据\\weights\\best_new.onnx";
            //var detector = new OnnxObjectDetector(detectOnnx, new string[] { "l" });
            //var results = detector.Detect("F:\\TestProgram\\Test\\手机拍摄\\mmexport1688016801491.jpg");
            //foreach (var result in results)
            //{
            //    Console.WriteLine($"类别: {detector.Labels[result.ClassId]}, " +
            //                     $"置信度: {result.Confidence:F2}, " +
            //                     $"位置: {result.Bounds}");
            //}

            #endregion

            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json", true, true);
            var configuration = builder?.Build();

            _host = CreateHostBuilder(args).Build();

            InitPlugins();
            InitSqlSugar();
            InitDapper(configuration);

            #region 测试贝塞尔曲线绘制

            //var bPath = new Common.Dto.BezierPath();
            //bPath.Source.X = 20;
            //bPath.Source.Y = 20;
            //bPath.Source.Status = "#FF0000";
            //bPath.Target.X = 200;
            //bPath.Target.Y = 20;
            //bPath.Target.Status = "#FF0000";
            //bPath.Vertices.Add(new Common.Dto.BezierPosition() { X = 20, Y = 100, IsControl = true });
            //bPath.Vertices.Add(new Common.Dto.BezierPosition() { X = 200, Y = 100, IsControl = true });
            //BezierUtils.BezierToImage(bPath);

            #endregion

            #region 后续会被删除或调整

            // 设置Token根密钥
            JwtHelper.Init();

            // 临时数据库中的用户
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

            // 启动Worker线程
            using (var serviceScope = _host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    // InitWorkers(services);
                }
                catch (Exception ex)
                {
                }
            }

            #endregion

            _logger.Debug("程序启动成功");

            _host.Run();
        }

        #region Plugin Func

        /// <summary>
        /// 从plugins文件夹中初始化插件
        /// </summary>
        protected static void InitPlugins()
        {
            _pluginManager = new PluginManager(_pluginPath);
            var num = _pluginManager.ControlPlugin(PluginOpt.启动, _pluginManager.GetPlugins);
            _logger.Debug(string.Format("一共启动插件{0}个,成功{1}个", _pluginManager.GetPlugins.Count, num));

            #region 测试内存增长问题

            //_pluginManager = new PluginManager(string.Empty);
            //_pluginManager.PluginRootPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "plugins");
            //for (int i = 0; i < 1000; i++)
            //{
            //    // 加载
            //    _pluginManager.AddPlugin("Ebos.Led.Plugin.RoadState", out Plugin newPlugin);
            //    // 启动
            //    if (newPlugin != null)
            //    {
            //        _pluginManager.ControlPlugin(PluginOpt.启动, new List<Plugin> { newPlugin });
            //    }
            //    // 卸载
            //    _pluginManager.ControlPlugin(PluginOpt.卸载, _pluginManager.GetPlugins);
            //}

            #endregion

            #region 文件监听

            _watcher = new FileSystemWatcher();
            _watcher.Path = _pluginPath;//设置监控的文件目录
            _watcher.Filter = "*.dll";//设置监控文件的类型"*.txt|*.doc|*.jpg"
            _watcher.Changed += new FileSystemEventHandler(OnProcess);
            //_watcher.Created += new FileSystemEventHandler(OnProcess);
            //_watcher.Deleted += new FileSystemEventHandler(OnProcess);
            //_watcher.Renamed += new RenamedEventHandler(OnRenamed);
            //_watcher.NotifyFilter = NotifyFilters.Attributes | NotifyFilters.CreationTime | NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.Security | NotifyFilters.Size;
            _watcher.NotifyFilter = NotifyFilters.LastAccess;
            _watcher.IncludeSubdirectories = true;//设置监控目录下的所有子目录
            _watcher.EnableRaisingEvents = true;//启动监控

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
        /// 初始化Dapper数据库连接，测试数据库使用
        /// </summary>
        /// <param name="configuration"></param>
        protected static void InitDapper(IConfigurationRoot configuration)
        {
            var connStr = configuration.GetSection("ConnectionStrings").GetValue<string>("DefaultConnectionString");
            DbProviderFactories.RegisterFactory("DmClientFactory", Dm.DmClientFactory.Instance); // 达梦，使用需要授权收费
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
            //_logger.WriteLine($"文件新建事件处理逻辑 {e.ChangeType}  {e.FullPath}  {e.Name}");
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
                        newPlugin.ControlWorkers(PluginOpt.启动);
                        _logger.Debug(string.Format("更新插件成功:{0}", dllName));
                    }
                }
                else
                {
                    if (_pluginManager.AddPlugin(dllName, out Plugin newPlugin))
                    {
                        newPlugin.ControlWorkers(PluginOpt.启动);
                        _logger.Debug(string.Format("启动插件成功:{0}", dllName));
                    }
                }
            }
            //_logger.WriteLine($"文件改变事件处理逻辑{e.ChangeType}  {e.FullPath}  {e.Name}");
        }

        private static void OnDeleted(object source, FileSystemEventArgs e)
        {
            //_logger.WriteLine($"文件删除事件处理逻辑{e.ChangeType}  {e.FullPath}   {e.Name}");
        }

        private static void OnRenamed(object source, RenamedEventArgs e)
        {
            //_logger.WriteLine($"文件重命名事件处理逻辑{e.ChangeType}  {e.FullPath}  {e.Name}");
        }

        #endregion

        static void InitWorkers(IServiceProvider services)
        {
            var taskThreadWorker = services.GetRequiredService<ITaskThreadWorker>();
            taskThreadWorker.Start();
        }

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