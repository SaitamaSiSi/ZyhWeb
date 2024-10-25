//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 10:05:51</date>
//------------------------------------------------------------------------------

using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zyh.Common.Filter.Web
{
    public class ApiActionFilter : IActionFilter, IExceptionFilter
    {
        public ApiActionFilter() { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            return;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            return;
        }

        public void OnException(ExceptionContext context)
        {
            return;
        }
    }
}
