using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zyh.Common.Entity.Condition;
using Zyh.Common.Net;
using Zyh.Common.Service.Services;

namespace Zyh.Web.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DictController : ControllerBase
    {
        private DataDictSqlService _dataDictService = new DataDictSqlService();

        public DictController()
        {
        }

        [HttpPost, Route("test"), AllowAnonymous]
        public ReqResult<string> Test()
        {
            return ReqResult<string>.Success(string.Empty);
        }

        [HttpPost, Route("create"), AllowAnonymous]
        public ReqResult<string> Create()
        {
            return ReqResult<string>.Success(string.Empty);
        }

        [HttpPost, Route("edit"), AllowAnonymous]
        public ReqResult<string> Edit()
        {
            return ReqResult<string>.Success(string.Empty);
        }

        [HttpPost, Route("delete"), AllowAnonymous]
        public ReqResult<string> Delete()
        {
            return ReqResult<string>.Success(string.Empty);
        }

        [HttpPost, Route("Enable"), AllowAnonymous]
        public ReqResult<string> Enable()
        {
            return ReqResult<string>.Success(string.Empty);
        }

        [HttpPost, Route("getPager"), AllowAnonymous]
        public ReqResult<string> GetPager([FromBody] DataDictCondition condition)
        {
            return ReqResult<string>.Success(string.Empty);
        }

        [HttpPost, Route("getCatgPager"), AllowAnonymous]
        public ReqResult<string> GetCatgPager([FromBody] DataDictCondition condition)
        {
            return ReqResult<string>.Success(string.Empty);
        }
    }
}
