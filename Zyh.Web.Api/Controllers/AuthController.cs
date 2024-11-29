using Microsoft.AspNetCore.Mvc;
using System.IO.Pipelines;
using Zyh.Common.Net;
using Zyh.Web.Api.Core;
using Zyh.Web.Api.Models;
using Zyh.Web.Api.Mysql;

namespace Zyh.Web.Api.Controllers
{
    /// <summary>
    /// 登录和权限相关接口
    /// </summary>
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
        public ReqResult<string> Logout([FromBody] LoginParams condition)
        {
            LoginManager.Logout(condition.username);
            return ReqResult<string>.Success("Logout");
        }

        [HttpPost, Route("login")]
        public ReqResult<LoginResult> Login([FromBody] LoginParams condition)
        {
            LoginResult loginResult = new LoginResult();
            loginResult.username = condition.username;

            if (!TempUsers.SignInClients.TryGetValue(condition.username, out _))
            {
                loginResult.desc = "用户不存在";
                return ReqResult<LoginResult>.Failed(loginResult);
            }

            if (!string.Equals(condition.password, "123456"))
            {
                loginResult.desc = "密码不正确";
                return ReqResult<LoginResult>.Failed(loginResult);
            }

            loginResult.userId = "0";
            loginResult.realName = condition.username.ToUpper();
            loginResult.accessToken = @"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MCwicGFzc3dvcmQiOiIxMjM0NTYiLCJyZWFsTmFtZSI6IlZiZW4iLCJyb2xlcyI6WyJzdXBlciJdLCJ1c2VybmFtZSI6InZiZW4iLCJpYXQiOjE3MjU5MzA3OTIsImV4cCI6MTcyNjUzNTU5Mn0.psoA46Gacz8lCLqHMsy_odZGsDuyjJI_7TIPFveySlw";


            LoginClient client = new LoginClient();
            client.userInfo.realName = loginResult.realName;
            client.userInfo.username = loginResult.username;
            client.userInfo.userId = loginResult.userId;
            switch (loginResult.username)
            {
                case "vben":
                    {
                        client.userInfo.roles.Add("super");
                        client.userCodes.AddRange(new string[] {
                            "AC_100100",
                        });
                        break;
                    }
                case "admin":
                    {
                        client.userInfo.roles.Add("admin");
                        client.userCodes.AddRange(new string[] {
                            "AC_100030",
                        });
                        break;
                    }
                default:
                    {
                        client.userInfo.roles.Add("user");
                        client.userCodes.AddRange(new string[] {
                            "AC_1000001"
                        });
                        break;
                    }
            }
            LoginManager.Login(client);

            return ReqResult<LoginResult>.Success(loginResult);
        }

        [HttpPost, Route("register")]
        public ReqResult<LoginResult> Register([FromBody] LoginParams condition)
        {
            LoginResult loginResult = new LoginResult();

            if (!TempUsers.SignInClients.TryGetValue(condition.username, out _))
            {
                loginResult.desc = "该用户已存在";
                return ReqResult<LoginResult>.Failed(loginResult);
            }

            TempUsers.SignInClients.TryAdd(condition.username, new UserInfo()
            {
                userId = condition.username,
                realName = condition.username.ToUpper()
            });

            loginResult.userId = "0";
            loginResult.realName = condition.username.ToUpper();
            loginResult.accessToken = @"eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6MCwicGFzc3dvcmQiOiIxMjM0NTYiLCJyZWFsTmFtZSI6IlZiZW4iLCJyb2xlcyI6WyJzdXBlciJdLCJ1c2VybmFtZSI6InZiZW4iLCJpYXQiOjE3MjU5MzA3OTIsImV4cCI6MTcyNjUzNTU5Mn0.psoA46Gacz8lCLqHMsy_odZGsDuyjJI_7TIPFveySlw";

            return ReqResult<LoginResult>.Success(loginResult);
        }

        [HttpPost, Route("codes")]
        public ReqResult<List<string>> Codes([FromBody] LoginParams condition)
        {
            List<string> codes = LoginManager.GetCodes(condition.username);
            return ReqResult<List<string>>.Success(codes);
        }
    }
}
