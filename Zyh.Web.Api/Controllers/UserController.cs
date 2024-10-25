using Microsoft.AspNetCore.Mvc;
using Zyh.Common.Net;
using Zyh.Web.Api.Core;
using Zyh.Web.Api.Models;

namespace Zyh.Web.Api.Controllers
{
    /// <summary>
    /// 登录用户信息相关接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;

        public UserController(ILogger<UserController> logger)
        {
            _logger = logger;
        }

        [HttpPost, Route("info")]
        public ReqResult<UserInfo> Info([FromBody] LoginParams condition)
        {
            UserInfo info = LoginManager.GetInfo(condition.username);
            return ReqResult<UserInfo>.Success(info);
        }
    }
}
