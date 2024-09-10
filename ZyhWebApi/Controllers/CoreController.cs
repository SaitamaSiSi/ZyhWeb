using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using ZyhWebApi.Models;

namespace ULIT.ICAPI.Controllers
{
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
        public string Post([FromBody] AuthCondition condition)
        {
            return @"{{""code"": 124, ""msg"": ""OkO""}}";
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
    }
}