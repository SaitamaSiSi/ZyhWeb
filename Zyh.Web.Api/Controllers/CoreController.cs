using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.IO;
using System.Reflection;
using Zyh.Common.Net;
using Zyh.Common.Security;
using Zyh.Web.Api.Models;

namespace Zyh.Web.Api.Controllers
{
    /// <summary>
    /// ������ؽӿ�
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CoreController : ControllerBase
    {
        private readonly ILogger<CoreController> _logger;

        public CoreController(ILogger<CoreController> logger)
        {
            _logger = logger;
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
            // ��ʼ�� ChromeDriver
            var dir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Chrome", "chromedriver", "win64");
            var dd = new DirectoryInfo(dir);
            var newDir = dd.GetDirectories().OrderBy(m => m.CreationTime).FirstOrDefault();
            ChromeDriverService driverService = ChromeDriverService.CreateDefaultService(newDir.FullName);

            //�رպ�ɫcmd����
            driverService.HideCommandPromptWindow = true;
            ChromeOptions options = new ChromeOptions();
            // ����ʾ�����
            options.AddArgument("--headless");
            // GPU���ٿ��ܻᵼ��Chrome���ֺ�����CPUռ���ʹ���,���Խ���
            options.AddArgument("--disable-gpu");
            //�����޺�ģʽ
            options.AddArgument("--incognito");
            //�ر�ɳ��
            options.AddArgument("no-sandbox");

            var chromeDriver = new ChromeDriver(driverService, options);

            try
            {
                chromeDriver.Manage().Window.Size = new System.Drawing.Size(500, 500);

                // HTML ������ַ���
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

                // ʹ�� ChromeDriver ���� HTML �����
                chromeDriver.Navigate().GoToUrl("data:text/html;charset=utf-8," + htmlString);
                chromeDriver.ExecuteScript(jsString);

                // ������Խ������������������ͼ��
                var result = chromeDriver.GetScreenshot();
                result.SaveAsFile("./test.png");
            }
            catch (Exception)
            {

            }
            finally
            {
                // �ͷ���Դ
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
                return StatusCode(200, ReqResult<string>.Failed("ʧ��1"));
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
                    // ��ȡ��׺��
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
                            return StatusCode(200, ReqResult<string>.Failed("ʧ��3"));
                        }

                        // hashֵ����
                        targetStream.Seek(0, SeekOrigin.Begin);
                        var HashValue = HashHelper.DoCompute("BLAKE2b", targetStream);
                        var Size = (int)targetStream.Length;
                    }
                    return StatusCode(200, ReqResult<string>.Success(contentDisposition.FileName.Value));
                }
                section = reader.ReadNextSectionAsync().GetAwaiter().GetResult();
            }
            return StatusCode(200, ReqResult<string>.Failed("ʧ��2"));
        }

    }
}