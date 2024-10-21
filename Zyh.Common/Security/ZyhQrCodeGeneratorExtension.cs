//------------------------------------------------------------------------------
// <copyright file="QrCodeGenerator.cs" company="CQ ULIT Co., Ltd.">
//    Copyright (c) 2024, Chongqing Youliang Science & Technology Co., Ltd. All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/12 14:10:44</date>
//------------------------------------------------------------------------------

using QRCoder;

namespace Zyh.Common.Security
{
    public class ZyhQrCodeGeneratorExtension : ZyhQrCodeGeneratorBase
    {
        public ZyhQrCodeGeneratorExtension() : base() { }

        /// <summary>
        /// 如果需要基于像素的图像 （=Bitmap） 并且正在使用 .NET Framework，请使用它。
        /// 如果您想在应用程序中显示 QR 码、将 QR 码保存为图像文件或将 QR 码作为下载提供，请使用它。
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Bitmap? GetQrCodeBitmap(System.Drawing.Bitmap? bitmap = null)
        {
            QRCode qrCode = new QRCode(_qrCodeData);
            if (bitmap != null)
            {
                return qrCode.GetGraphic(
                    pixelsPerModule: 20, // 绘制每个黑白模块的像素大小
                    darkColor: System.Drawing.Color.Black, // 黑色模块颜色
                    lightColor: System.Drawing.Color.White, // 白色模块颜色
                    icon: bitmap, // icon图标
                    iconSizePercent: 15, // 图标占比 1-99
                    iconBorderWidth: 6, // 图标周围绘制的边框的宽度,最小1
                    drawQuietZones: true, // 整个QR码周围绘制白色边框
                    iconBackgroundColor: null // 图标背景颜色,为null则使用白色模块值,仅呈现图标背景,只有在icon!=null和iconBorderWidth>0才生效
                );
            }
            else
            {
                // 绘制每个黑白模块的像素大小,黑色模块颜色,白色模块颜色,整个QR码周围绘制白色边框
                return qrCode.GetGraphic(20, "#000000", "#FFFFFF", true);
            }
        }

        /// <summary>
        /// 通常在 Web 和数据库环境中非常有用。
        /// 您可以将 Base64 图像嵌入到 html 代码中，通过 HTTP 请求轻松传输它们或将它们轻松写入数据库。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public string GetBase64QRCodeStringWithBitmap(Base64QRCode.ImageType type = Base64QRCode.ImageType.Png, System.Drawing.Bitmap? bitmap = null)
        {
            Base64QRCode qrCode = new Base64QRCode(_qrCodeData);
            if (bitmap != null)
            {
                return qrCode.GetGraphic(
                    pixelsPerModule: 20, // 绘制每个黑白模块的像素大小
                    darkColor: System.Drawing.Color.Black,
                    lightColor: System.Drawing.Color.White,
                    icon: bitmap, // icon图标
                    iconSizePercent: 15, // 图标占比 1-99
                    iconBorderWidth: 6, // 图标周围绘制的边框的宽度,最小1
                    drawQuietZones: true, // 整个QR码周围绘制白色边框
                    imgType: type // Base64 字符串编码的图片格式
                );
            }
            else
            {
                return qrCode.GetGraphic(
                    pixelsPerModule: 20, // 绘制每个黑白模块的像素大小
                    darkColorHtmlHex: "#000000",
                    lightColorHtmlHex: "#FFFFFF",
                    drawQuietZones: true, // 整个QR码周围绘制白色边框
                    imgType: type // Base64 字符串编码的图片格式
                );
            }
        }

        /// <summary>
        /// GetBase64QRCodeString方法的扩展，转换为HTML
        /// </summary>
        /// <param name="type"></param>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public string GetBase64QRCodeStringWithBitmapExtension(Base64QRCode.ImageType type = Base64QRCode.ImageType.Png, System.Drawing.Bitmap? bitmap = null)
        {
            string qrCodeImageAsBase64 = GetBase64QRCodeStringWithBitmap(type, bitmap);
            return $"<img alt=\"Embedded QR Code\" src=\"data:image/{type.ToString().ToLower()};base64,{qrCodeImageAsBase64}\" />";
        }
    }
}
