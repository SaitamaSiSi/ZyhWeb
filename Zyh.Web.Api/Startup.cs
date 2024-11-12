using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using System.Diagnostics;
using System.Text.Json.Serialization;
using Zyh.Common.Filter.Web;

namespace Zyh.Web.Api
{
    public class Startup
    {
        public const string wwwroot = "wwwroot";
        public const string resources = "resources";
        public IConfiguration Configuration { get; }

        public IConfiguration OcelotConfiguration { get; set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false;
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = null;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            });
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
                    TermsOfService = new Uri("https://zyh.com"),
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
                    && !context.Request.Path.StartsWithSegments("/tips", StringComparison.OrdinalIgnoreCase)
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

            //var fileServerOptions = new FileServerOptions()
            //{
            //    EnableDefaultFiles = true,
            //    // 是否显示文件夹
            //    EnableDirectoryBrowsing = false,
            //    RequestPath = "/tips/api/file/v1",
            //    FileProvider = new PhysicalFileProvider("")
            //};
            //fileServerOptions.StaticFileOptions.ServeUnknownFileTypes = true;
            //app.UseFileServer(fileServerOptions);
        }
    }
}

