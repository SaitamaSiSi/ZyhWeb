using Microsoft.AspNetCore.Mvc;

namespace ULIT.ICAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoreController : ControllerBase
    {
        private readonly ILogger<CoreController> _logger;

        public CoreController(ILogger<CoreController> logger)
        {
            _logger = logger;
        }

        [HttpGet, Route("get")]
        public string Get()
        {
            return "get";
        }

        [HttpPost, Route("post")]
        public string Post()
        {
            return "post";
        }
    }
}