using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using OpenQA.Selenium.Chrome;
using System.Net;
using System.Reflection;
using Zyh.Common.IO;
using Zyh.Common.Net;
using Zyh.Common.Security;
using Zyh.Web.Api.Models;

namespace Zyh.Web.Api.Controllers
{
    /// <summary>
    /// 测试相关接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CoreController : ControllerBase
    {
        private readonly ILogger<CoreController> _logger;
        private readonly WebSetting _webSetting;
        private readonly BufferPool _bufferPool = new BufferPool();

        public CoreController(ILogger<CoreController> logger, IOptionsMonitor<WebSetting> setting)
        {
            _logger = logger;
            _webSetting = setting.CurrentValue;
        }

        [HttpGet, Route("get")]
        public string Get()
        {
            return "get";
        }

        [HttpPost, Route("post")]
        public ReqResult<string> Post([FromBody] LoginParams condition)
        {
            return ReqResult<string>.Success("Test");
            //            return @$"{{
            //  ""code"": 0,
            //  ""data"": {{
            //    ""code"": 124,
            //    ""msg"": ""OK"",
            //  }},
            //  ""error"": null,
            //  ""message"": ""ok""
            //}}";
        }

        [HttpPost, Route("testChrome")]
        public string TestChrome()
        {
            return "";
            // 初始化 ChromeDriver
            var dir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Chrome", "chromedriver", "win64");
            var dd = new DirectoryInfo(dir);
            var newDir = dd.GetDirectories().OrderBy(m => m.CreationTime).FirstOrDefault();
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(newDir.FullName);

            //关闭黑色cmd窗口
            driverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            // 不显示浏览器
            options.AddArgument("--headless");
            // GPU加速可能会导致Chrome出现黑屏及CPU占用率过高,所以禁用
            options.AddArgument("--disable-gpu");
            //启用无痕模式
            options.AddArgument("--incognito");
            //关闭沙盒
            options.AddArgument("no-sandbox");

            var chromeDriver = new ChromeDriver(driverService, options);

            try
            {
                chromeDriver.Manage().Window.Size = new System.Drawing.Size(500, 500);

                // HTML 代码段字符串
                string htmlString = @$"
<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Test Page</title>
</head>
<body>
    <h1>Welcome to the Test Page</h1>
    <button id=""alertButton"">Click me to see an alert</button>
</body>
</html>
";

                string jsString = @$"
document.getElementById('alertButton').textContent = 'Button Text Changed by JavaScript';
";

                // 使用 ChromeDriver 加载 HTML 代码段
                chromeDriver.Navigate().GoToUrl("data:text/html;charset=utf-8," + htmlString);
                chromeDriver.ExecuteScript(jsString);

                // 这里可以进行其他操作，例如截图等
                var result = chromeDriver.GetScreenshot();
                result.SaveAsFile("./test.png");
            }
            catch (Exception)
            {

            }
            finally
            {
                // 释放资源
                chromeDriver.Quit();
            }

            return "";
        }

        [HttpPost, Route("upload")]
        public IActionResult Upload([FromForm] UploadParams param)
        {
            var request = HttpContext.Request;

            if (!request.HasFormContentType ||
                !MediaTypeHeaderValue.TryParse(request.ContentType, out var mediaTypeHeader) ||
                string.IsNullOrEmpty(mediaTypeHeader.Boundary.Value))
            {
                return StatusCode(200, ReqResult<string>.Failed("失败1"));
            }

            var reader = new MultipartReader(mediaTypeHeader.Boundary.Value, request.Body, 64 * 1024);
            var section = reader.ReadNextSectionAsync().GetAwaiter().GetResult();
            while (section != null)
            {
                var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition,
                            out var contentDisposition);
                if (hasContentDispositionHeader && contentDisposition.DispositionType.Equals("form-data") &&
                            !string.IsNullOrEmpty(contentDisposition.FileName.Value))
                {
                    // 获取后缀名
                    var suffix = Path.GetExtension(contentDisposition.FileName.Value);
                    var Name = contentDisposition.FileName.Value;
                    using (var targetStream = System.IO.File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Name)))
                    {
                        section.Body.CopyTo(targetStream, 64 * 1024);

                        targetStream.Seek(0, SeekOrigin.Begin);
                        var md5Helper = new MD5FileStreamProcessor();
                        var md5 = md5Helper.CalculateMD5Hash(targetStream);
                        if (!string.Equals(md5, param.Md5, StringComparison.OrdinalIgnoreCase))
                        {
                            return StatusCode(200, ReqResult<string>.Failed("失败3"));
                        }

                        // hash值计算
                        targetStream.Seek(0, SeekOrigin.Begin);
                        var HashValue = HashHelper.DoCompute("BLAKE2b", targetStream);
                        var Size = (int)targetStream.Length;
                    }
                    return StatusCode(200, ReqResult<string>.Success(contentDisposition.FileName.Value));
                }
                section = reader.ReadNextSectionAsync().GetAwaiter().GetResult();
            }
            return StatusCode(200, ReqResult<string>.Failed("失败2"));
        }

        [HttpGet, Route("getByName/{name}"), AllowAnonymous]
        public IActionResult GetByName(string name)
        {
            var msg = string.Empty;
            var rspSize = 0L;
            var readBufferSize = 65536;

            Response.ContentType = "video/mp4";
            Response.StatusCode = (int)HttpStatusCode.OK;
            //Response.Headers.Add("Access-Control-Allow-Origin", "*");

            try
            {
                string filePath = Path.Combine(_webSetting.ResourceDir, name);
                using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 64 * 1024))
                {
                    if (HttpContext.RequestAborted.IsCancellationRequested)
                    {
                        return new EmptyResult();
                    }
                    bool b = false;
                    rspSize = ResponseFileByPool(stream, stream.Length, readBufferSize, ref b);
                }

                /*
                string json = "测试文本内容";
                Byte[] buffer = new Byte[json.Length];
                buffer = Encoding.Default.GetBytes(json);
                Response.Body.Write(buffer, 0, buffer.Length);
                Response.Body.Flush();
                */
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            finally
            {
            }

            return new EmptyResult();
        }

        /// <summary>
        /// 线程池方式获取文件内容
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="blockLength"></param>
        /// <param name="readBufferSize"></param>
        /// <param name="reqStatus"></param>
        /// <returns></returns>
        private Int64 ResponseFileByPool(Stream stream, Int64 blockLength, Int32 readBufferSize, ref bool reqStatus)
        {
            const Int32 Size128KB = 128 * 1024;//128KB
            const Int32 Size1536KB = 128 * 12 * 1024;//1.5MB
            Int64 offset = 0;
            Int64 bufferSize = readBufferSize;//64KB
            var index = 0;
            while (offset < blockLength)
            {
                if (HttpContext.RequestAborted.IsCancellationRequested)
                {
                    reqStatus = false;
                    break;
                }

                bufferSize = GetRequiredSize(blockLength, offset, readBufferSize, Size128KB, Size1536KB);

                if (bufferSize <= 0)
                {
                    break;
                }

                Byte[] buffer = null;

                if (bufferSize > readBufferSize)
                {
                    buffer = _bufferPool.Get(bufferSize);
                }
                else
                {
                    buffer = new Byte[bufferSize];
                }

                try
                {
                    index++;
                    var readedCount = stream.Read(buffer, 0, buffer.Length);
                    offset += readedCount;
                    Response.Body.Write(buffer, 0, readedCount);
                    Response.Body.Flush();
                }
                catch
                {
                    throw;
                }
                finally
                {
                    if (bufferSize > readBufferSize)
                    {
                        _bufferPool.Return(buffer);
                    }
                }
            }

            return offset;
        }

        /// <summary>
        /// 根据文件剩余大小分配buffer大小
        /// </summary>
        /// <param name="blockLength"></param>
        /// <param name="offset"></param>
        /// <param name="minSize"></param>
        /// <param name="avgSize"></param>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        private Int64 GetRequiredSize(Int64 blockLength, Int64 offset, Int32 minSize, Int32 avgSize, Int32 maxSize)
        {
            Int64 value = blockLength - offset;

            if (value <= 0L)
            {
                return 0L;
            }

            if (value < avgSize)
            {
                if (value >= minSize)
                {
                    return minSize;
                }

                return value;
            }

            var residue = value % avgSize;

            value = value - residue;

            if (value >= maxSize)
            {
                return maxSize;
            }

            return value;
        }

    }
}