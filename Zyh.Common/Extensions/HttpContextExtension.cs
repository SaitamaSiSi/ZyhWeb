//------------------------------------------------------------------------------
// <copyright file="HttpContextExtension.cs" company="Zyh">
//    Copyright (c) 2024, Zyh All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 13:52:08</date>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Http;

namespace Zyh.Common.Extensions
{
    public static class HttpContextExtension
    {
        public static string GetClientIP(this HttpContext context)
        {
            if (context == null
                || context.Connection == null
                || context.Connection.RemoteIpAddress == null)
            {
                return string.Empty;
            }

            var ip = context.Connection.RemoteIpAddress.MapToIPv4();
            if (ip == null)
            {
                return string.Empty;
            }

            return ip.ToString();
        }
    }
}
