//------------------------------------------------------------------------------
// <copyright file="WebAuthFilter.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 9:57:14</date>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;

namespace Zyh.Common.Filter.Web
{
    public class AuthFilter : IAuthorizationFilter, IOrderedFilter
    {
        public int Order { get; }

        public AuthFilter() { }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // 使文件流上传可再读取
            context.HttpContext.Request.EnableBuffering();

            // ...

            return;
        }
    }
}
