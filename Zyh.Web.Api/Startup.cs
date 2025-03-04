using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
using Zyh.Common.Filter.Web;
using Zyh.Web.Api.Models;
using Zyh.Web.Api.Worker;

namespace Zyh.Web.Api
{
    public class Startup
    {
        public const string wwwroot = "wwwroot";
        public const string resources = "resources";
        public IConfiguration Configuration { get; }
        public WebSetting Setting { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            Setting = new WebSetting();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITaskThreadWorker, TaskThreadWorker>();

            //上传文件大小限制Kestrel设置
            services.Configure<KestrelServerOptions>(options =>
            {
                // Set the limit to 256 MB
                options.Limits.MaxRequestBodySize = long.MaxValue;
            });
            //上传文件大小限制IIS设置
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = long.MaxValue;

            });
            services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(opt =>
            {
                opt.MultipartBodyLengthLimit = long.MaxValue;
                opt.ValueLengthLimit = int.MaxValue;
            });

            services.Configure<WebSetting>(Configuration.GetSection(nameof(WebSetting)));
            Configuration.Bind(nameof(WebSetting), this.Setting);

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
                options.Filters.Add(typeof(AuthFilter));
                options.Filters.Add(typeof(ApiActionFilter));
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
            services.AddHttpContextAccessor();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1.0",
                    Title = "WebApi",
                    Description = @"测试",
                    TermsOfService = new Uri("https://h.cqzyh.cn:2083"),
                    Contact = new OpenApiContact
                    {
                        Name = "zyh",
                        Email = "zyh@live.cn",
                        Url = new Uri("https://github.com/SaitamaSiSi"),
                    }
                });
            });

            services.Configure<KestrelServerOptions>(x => x.AllowSynchronousIO = true)
                .Configure<IISServerOptions>(x => x.AllowSynchronousIO = true);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options =>
            {
                options.ExpireTimeSpan = TimeSpan.FromMinutes(10);
                options.Cookie.Name = "zyh-auth";
                options.SlidingExpiration = true;
            });

            services.AddRateLimiter(options =>
            {
                #region 全局限流

                //options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                //    RateLimitPartition.GetFixedWindowLimiter("global", _ => new()
                //    {
                //        PermitLimit = 600,    // 时间窗口内允许的最大请求数
                //        Window = TimeSpan.FromMinutes(1) // 时间窗口长度
                //    }));

                #endregion

                #region 终结点限流
                // 对需要的接口加上[EnableRateLimiting("fixePolicy")]标记，每个接口单独统计次数

                // 固定窗口，在固定时间窗口（如1分钟）内统计请求数，超过阈值则拒绝后续请求
                options.AddPolicy("fixePolicy", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Request.Path, // 根据请求路径来限流
                        _ => new()
                        {
                            PermitLimit = 3,
                            Window = TimeSpan.FromSeconds(60)
                        }));

                // 滑动窗口，将时间窗口划分为多个子段（如1分钟分为6个10秒段），窗口滑动时合并过期段的请求数
                options.AddPolicy("slidingPolicy",
                    context => RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: context.Connection.RemoteIpAddress?.ToString(), // 根据请求IP来限流
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 10,                   // 每分钟最多3次请求
                            Window = TimeSpan.FromMinutes(1),  // 窗口总时长1分钟
                            SegmentsPerWindow = 6,             // 将窗口分为6段（每段10秒）
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                            QueueLimit = 0                     // 不排队，直接拒绝
                        }));

                // 令牌桶算法，以固定速率生成令牌，请求需获取令牌才能处理，桶满时丢弃新令牌
                options.AddTokenBucketLimiter("tokenPolicy", opt =>
                {
                    opt.TokenLimit = 100;
                    opt.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                    opt.TokensPerPeriod = 20; // 每10秒补充20个令牌
                });

                // 并发限流，限制同时处理的请求数（如最多100个并发），超出直接拒绝
                options.AddConcurrencyLimiter("concurrentPolicy", opt =>
                {
                    opt.PermitLimit = 100;
                });

                #endregion

                // 自定义限流返回状态码
                options.OnRejected = (context, _) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    return new ValueTask();
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors(options =>
            {
                //options.AllowAnyOrigin();
                options.SetIsOriginAllowed(host => true);
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowCredentials();
            });

            // 建立WebSocket服务器
            var webSocketOptions = new WebSocketOptions()
            {
                KeepAliveInterval = TimeSpan.FromSeconds(120),
            };
            app.UseWebSockets(webSocketOptions);

#if DEBUG
            app.UseSwagger(options =>
            {
                options.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebApi v1"));

#endif

            app.UseDefaultFiles();
            app.UseStaticFiles();
            InitFileServer(app, env);

            app.UseRouting();

            //此处添加自定义中间件操作，捕获收到请求前和处理请求后操作。其中next()为分段。
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Request.Method == HttpMethods.Get
                    && context.Request.Path != null
                    && !context.Request.Path.StartsWithSegments("/jocker", StringComparison.OrdinalIgnoreCase)
                    && context.Response.StatusCode == StatusCodes.Status404NotFound)
                {
                    context.Response.ContentType = "text/html";
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    await context.Response.SendFileAsync(Path.Combine(env.WebRootPath, "index.html"));
                    return;
                }
            });


            //启用身份验证 需要在app.UseAuthorization();之前启用身份验证，因为需要进行身份验证后才能进行授权。
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter(); // 限流器在认证之后

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapControllers();
            });

            app.UseMvc();

            Trace.WriteLine($"--程序启动--");
        }

        void InitFileServer(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var rootPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            if (string.IsNullOrWhiteSpace(env.WebRootPath))
            {
                env.WebRootPath = Path.Combine(rootPath, wwwroot);
            }

            var physicalPath = Configuration.GetSection("WebSetting").GetValue<string>("ResourceDir");
            if (Directory.Exists(physicalPath))
            {
                var fileServerOptions = new FileServerOptions()
                {
                    EnableDefaultFiles = true,
                    // 是否显示文件夹
                    EnableDirectoryBrowsing = false,
                    RequestPath = "/api/file/v1",
                    FileProvider = new PhysicalFileProvider(physicalPath)
                };
                fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
                app.UseFileServer(fileServerOptions);
            }
        }
    }
}

