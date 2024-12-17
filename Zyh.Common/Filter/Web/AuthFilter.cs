//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 9:57:14</date>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;
using Zyh.Common.Net;
using Microsoft.AspNetCore.Mvc;
using Zyh.Common.Security;
using System;

namespace Zyh.Common.Filter.Web
{
    public class AuthFilter : IAuthorizationFilter, IOrderedFilter
    {
        public int Order { get; }

        public AuthFilter() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null || context.Filters == null || context.HttpContext == null)
            {
                return;
            }

            if (context.Filters.Any(m => m is AllowAnonymousFilter))
            {
                return;
            }

            // 使文件流上传可再读取
            context.HttpContext.Request.EnableBuffering();

            // 验证AccessToken
            string authorization = context.HttpContext.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorization))
            {
                // AccessToken不存在，重新登录
                context.Result = new JsonResult(new BaseResult(ResultStatus.Failed, 0x0401002));
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            // 认证过期
            if (!JwtHelper.CheckToken(authorization, "vben"))
            {
                // AccessToken过期，重新请求refresh
                context.Result = new JsonResult(new BaseResult(ResultStatus.Failed, 0x0401001));
                context.HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            context.HttpContext.Request.Body.Position = 0;
        }
    }
}
