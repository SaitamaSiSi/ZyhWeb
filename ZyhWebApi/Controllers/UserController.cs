using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ULIT.ICAPI.Controllers;

namespace ZyhWebApi.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("info")]
        public string Info()
        {
            string json = @$"{{
  ""code"": 0,
  ""data"": {{
    ""id"": 0,
    ""realName"": ""Vben"",
    ""roles"": [
      ""super""
    ],
    ""username"": ""vben""
  }},
  ""error"": null,
  ""message"": ""ok""
}}";
            return json;
        }
    }
}
