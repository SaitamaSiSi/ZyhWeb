using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zyh.Common.Net;
using Zyh.Data.Entity.Condition;
using Zyh.Data.Entity.Core;
using Zyh.Data.Entity.OpenGauss;
using Zyh.Data.Service.OpenGauss;

namespace Zyh.Web.Api.Controllers
{
    /// <summary>
    /// openGauss数据库操作相关接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OpenGaussController : ControllerBase
    {
        private OpenGaussConfig _config { get; set; }

        private LedEquipService _ledEquipService { get; set; }

        public OpenGaussController()
        {
            _config = new OpenGaussConfig()
            {
                Host = "192.168.100.188",
                Port = 5432,
                Database = "db_led",
                UserId = "test",
                UserPwd = "test@123",
            };
            _ledEquipService = new LedEquipService(_config.GetConnectString());
        }

        [HttpPost, Route("findAll"), AllowAnonymous]
        public ReqResult<List<LedEquipEntity>> FindAll()
        {
            List<LedEquipEntity> entities = _ledEquipService.FindAll();
            return ReqResult<List<LedEquipEntity>>.Success(entities);
        }

        [HttpPost, Route("getPager"), AllowAnonymous]
        public ReqResult<List<LedEquipEntity>> GetPager([FromBody] LedEquipCondition condition)
        {
            List<LedEquipEntity> entities = _ledEquipService.GetPager(condition);
            return ReqResult<List<LedEquipEntity>>.Success(entities);
        }

        [HttpPost, Route("get"), AllowAnonymous]
        public ReqResult<LedEquipEntity> Get([FromBody] LedEquipCondition condition)
        {
            LedEquipEntity entity = _ledEquipService.Get(condition);
            return ReqResult<LedEquipEntity>.Success(entity);
        }

        [HttpPost, Route("insert"), AllowAnonymous]
        public ReqResult<string> Insert([FromBody] LedEquipEntity entity)
        {
            bool bFlag = _ledEquipService.Insert(entity);
            return bFlag ? ReqResult<string>.Success("Insert Successfully") : ReqResult<string>.Failed("Insert Failed");
        }

        [HttpPost, Route("update"), AllowAnonymous]
        public ReqResult<string> Update([FromBody] LedEquipEntity entity)
        {
            bool bFlag = _ledEquipService.Update(entity);
            return bFlag ? ReqResult<string>.Success("Update Successfully") : ReqResult<string>.Failed("Update Failed");
        }

        [HttpPost, Route("delete"), AllowAnonymous]
        public ReqResult<string> Delete([FromBody] LedEquipCondition condition)
        {
            bool bFlag = _ledEquipService.Delete(condition);
            return bFlag ? ReqResult<string>.Success("Delete Successfully") : ReqResult<string>.Failed("Delete Failed");
        }
    }
}
