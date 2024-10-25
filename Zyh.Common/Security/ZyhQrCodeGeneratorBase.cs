//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/14 10:01:47</date>
//------------------------------------------------------------------------------

using OtpNet;
using QRCoder;
using static QRCoder.SvgQRCode;

namespace Zyh.Common.Security
{
    /// <summary>
    /// QrCode助手
    /// </summary>
    public class ZyhQrCodeGeneratorBase
    {
        protected string SECRET_BASE = "RLX5E3R2OHRPJWNHSYTAFVM4IARIOIPO";
        protected int TIME_SPAN = 30;
        protected int PWD_LENGTH = 6;
        protected OtpHashMode HASH_MODE = OtpHashMode.Sha512;
        protected readonly Totp _totp;
        protected QRCodeData _qrCodeData;

        public ZyhQrCodeGeneratorBase(string user = "zyh@test.com", string issuer = "Zyh Co")
        {
            using QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            _qrCodeData = qRCodeGenerator.CreateQrCode(
                new OtpUri(
                    schema: OtpType.Totp,
                    secret: SECRET_BASE,
                    user: user,
                    issuer: issuer,
                    algorithm: OtpHashMode.Sha512
                ).ToString(),
                QRCodeGenerator.ECCLevel.Q
            );

            _totp = new Totp(secretKey: Base32Encoding.ToBytes(SECRET_BASE), step: TIME_SPAN, mode: HASH_MODE, totpSize: PWD_LENGTH);
        }

        /// <summary>
        /// 适用于 Web、Retro 和 Console 环境。
        /// 通过使用此编码器，您可以获得一个编码为字符串的 QR 码，该字符串仅由字符构建而成。
        /// 默认情况下，使用 UTF-8 块符号。
        /// </summary>
        /// <returns></returns>
        public string GetAsciiQRCodeString()
        {
            AsciiQRCode qrCode = new AsciiQRCode(_qrCodeData);
            return qrCode.GetGraphic(
                repeatPerModule: 1, // 每个模块重复的 darkColorString/whiteSpaceString 的数量。必须为 1 或更大
                darkColorString: "#000000",
                whiteSpaceString: "#FFFFFF",
                drawQuietZones: true, // 整个QR码周围绘制白色边框
                endOfLine: "\n" // 行尾分隔符
            );
        }

        /// <summary>
        /// 通常在 Web 和数据库环境中非常有用。
        /// 您可以将 Base64 图像嵌入到 html 代码中，通过 HTTP 请求轻松传输它们或将它们轻松写入数据库。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public string GetBase64QRCodeString(Base64QRCode.ImageType type = Base64QRCode.ImageType.Png)
        {
            Base64QRCode qrCode = new Base64QRCode(_qrCodeData);
            return qrCode.GetGraphic(
                pixelsPerModule: 20, // 绘制每个黑白模块的像素大小
                darkColorHtmlHex: "#000000",
                lightColorHtmlHex: "#FFFFFF",
                drawQuietZones: true, // 整个QR码周围绘制白色边框
                imgType: type // Base64 字符串编码的图片格式
            );
        }

        /// <summary>
        /// GetBase64QRCodeString方法的扩展，转换为HTML
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public string GetBase64QRCodeStringExtension(Base64QRCode.ImageType type = Base64QRCode.ImageType.Png)
        {
            string qrCodeImageAsBase64 = GetBase64QRCodeString(type);
            return $"<img alt=\"Embedded QR Code\" src=\"data:image/{type.ToString().ToLower()};base64,{qrCodeImageAsBase64}\" />";
        }

        /// <summary>
        /// 当使用 QRCoder 的 PCL 版本时（例如在移动项目中），这是除 PngByteQRCode 之外唯一可用的渲染器。
        /// 它以字节数组的形式返回 QR 码，其中包含位图图形。
        /// 由于许多提供 PCL 库的平台具有不同的 Bitmap 类实现，但几乎任何平台都可以从 byte[] 创建图像，因此这是跨平台处理图像的某种方法。
        /// </summary>
        /// <returns></returns>
        public byte[] GetBitmapByteQRCodeBytes()
        {
            BitmapByteQRCode qrCode = new BitmapByteQRCode(_qrCodeData);
            return qrCode.GetGraphic(
                pixelsPerModule: 20,
                darkColorHtmlHex: "#CCCCCC",
                lightColorHtmlHex: "#444444"
            );
        }

        /// <summary>
        /// 当使用 QRCoder 的 PCL 版本时（例如，在移动项目中），除了 BitmapByteQRCode 之外，这是可用的渲染器。
        /// 它将 QR 码作为包含 PNG 图形的字节数组返回。
        /// 由于许多提供 PCL 库的平台具有不同的 Imaging 类实现，但几乎任何平台都可以从 byte[] 创建图像（或将字节数组写入文件），因此这是跨平台处理图像的某种方法。
        /// </summary>
        /// <param name="dark"></param>
        /// <param name="light"></param>
        /// <returns></returns>
        public byte[] GetPngByteQRCodeBytes(byte[] dark, byte[] light)
        {
            PngByteQRCode qrCode = new PngByteQRCode(_qrCodeData);
            return qrCode.GetGraphic(
                pixelsPerModule: 20,
                darkColorRgba: dark, // RGB（A） 数组
                lightColorRgba: light, // RGB（A） 数组
                drawQuietZones: true // 整个QR码周围绘制白色边框
            );
        }

        /// <summary>
        /// 如果您想打印大尺寸的 QR 码或处理可扩展（从屏幕尺寸的角度来看）Web 应用程序，请使用此选项。SvgQRCode 返回一个可缩放的矢量图形，该图形本质上永远不会变得模糊。
        /// </summary>
        /// <returns></returns>
        public string GetSvgQRCodeBytes(SvgLogo? logo = null)
        {
            SvgQRCode qrCode = new SvgQRCode(_qrCodeData);
            return qrCode.GetGraphic(
                pixelsPerModule: 20,
                darkColorHex: "#000000",
                lightColorHex: "#FFFFFF",
                drawQuietZones: true,
                sizingMode: SizingMode.WidthHeightAttribute,
                logo: logo
            );
        }

        public bool VertyQrCode(string userTotp)
        {
            return _totp.VerifyTotp(userTotp, out _);
        }
    }
}
