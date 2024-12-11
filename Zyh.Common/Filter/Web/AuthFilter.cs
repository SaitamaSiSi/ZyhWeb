//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 9:57:14</date>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Authorization;

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

            // ...
            string authorization = context.HttpContext.Request.Headers["Authorization"];

            return;
        }
    }
}
