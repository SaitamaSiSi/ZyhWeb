using Microsoft.AspNetCore.Mvc;
using Zyh.Common.Net;
using Zyh.Common.Security;
using Zyh.Web.Api.Models;

namespace Zyh.Web.Api.Controllers
{
    /// <summary>
    /// Qrcode二维码相关接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class QrCodeController : ControllerBase
    {
        private readonly ILogger<QrCodeController> _logger;
        private ZyhQrCodeGeneratorExtension _qrCodeGenerator = new();

        public QrCodeController(ILogger<QrCodeController> logger)
        {
            _logger = logger;
        }

        [HttpPost, Route("getQrcode")]
        public ReqResult<QrcodeResult> GetQrcode([FromBody] QrcodeParams @params)
        {
            QrcodeResult result = new();
            if (@params != null)
            {
                result.Url = _qrCodeGenerator.GetBase64QRCodeString();
            }
            return ReqResult<QrcodeResult>.Success(result);
        }

        [HttpPost, Route("vertyQrcode")]
        public ReqResult<QrcodeResult> VertyQrcode([FromBody] QrcodeParams @params)
        {
            QrcodeResult result = new();
            if (@params != null)
            {
                result.IsVerty = _qrCodeGenerator.VertyQrCode(@params.VertyValue.ToString());
            }
            return ReqResult<QrcodeResult>.Success(result);
        }
    }
}
