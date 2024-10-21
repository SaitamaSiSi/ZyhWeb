using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZyhWebApi.Models;

namespace ZyhWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> _logger;

        public AuthController(ILogger<AuthController> logger)
        {
            _logger = logger;
        }

        [HttpPost, Route("logout")]
        public string Logout([FromBody] AuthCondition condition)
        {
            string json = @$"{{
  ""code"": 0,
  ""data"": """",
  ""error"": null,
  ""message"": ""ok""
}}";
            return json;
        }

        [HttpPost, Route("login")]
        public string Login([FromBody] AuthCondition condition)
        {
            string json = @$"{{
  ""code"": 0,
  ""data"": {{
    ""id"": 0,
    ""password"": ""123456"",
    ""realName"": ""Vben"",
    ""roles"": [
      ""super""
    ],
    ""username"": ""vben"",
    ""accessToken"": ""eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MCwicGFzc3dvcmQiOiIxMjM0NTYiLCJyZWFsTmFtZSI6IlZiZW4iLCJyb2xlcyI6WyJzdXBlciJdLCJ1c2VybmFtZSI6InZiZW4iLCJpYXQiOjE3MjU5MzA3OTIsImV4cCI6MTcyNjUzNTU5Mn0.psoA46Gacz8lCLqHMsy_odZGsDuyjJI_7TIPFveySlw""
  }},
  ""error"": null,
  ""message"": ""ok""
}}";
            return json;
        }

        [HttpGet, Route("codes")]
        public string Codes()
        {
            string json = @$"{{
  ""code"": 0,
  ""data"": [
    ""AC_100100"",
    ""AC_100110"",
    ""AC_100120"",
    ""AC_100010""
  ],
  ""error"": null,
  ""message"": ""ok""
}}";
            return json;
        }
    }
}
