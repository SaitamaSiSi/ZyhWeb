//------------------------------------------------------------------------------
// <copyright file="ImageUtils.cs" company="CQ ULIT Co., Ltd.">
//    Copyright (c) 2024, Chongqing Youliang Science & Technology Co., Ltd. All rights reserved.
// </copyright>
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/12 14:50:53</date>
//------------------------------------------------------------------------------

using SkiaSharp;
using Svg.Skia;
using System;
using System.IO;
using System.Net;
using System.Xml;
using SixLabors.ImageSharp.Processing;

namespace Zyh.Common.Drawing
{
    public class ImageUtils
    {
        /// <summary>
        /// 缩放图片大小，不包含svg，gif等特殊图片
        /// </summary>
        /// <param name="byteImageIn">图片数据</param>
        /// <param name="newWidth">新宽</param>
        /// <param name="newHeight">新高</param>
        /// <returns></returns>
        public static byte[] CompressPicByImage(byte[] byteImageIn, int newWidth = 200, int newHeight = 200)
        {
            if (byteImageIn == null)
            {
                throw new ArgumentNullException("byteImageIn");
            }

            int quality = 100; //质量为[SKFilterQuality.Medium]结果的100%
            using (var inputMS = new MemoryStream(byteImageIn))
            {
                using var inputStream = new SKManagedStream(inputMS);
                using var original = SKBitmap.Decode(inputStream);
                using (var resized = original.Resize(new SKImageInfo(newWidth, newHeight), SKFilterQuality.High))
                {
                    if (resized != null)
                    {
                        using var image = SKImage.FromBitmap(resized);
                        using (var outMS = new MemoryStream())
                        {
                            image.Encode(SKEncodedImageFormat.Png, quality).SaveTo(outMS);
                            return outMS.ToArray();
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 设置GIF大小，Linux中可用
        /// </summary>
        /// <param name="byteImageIn">GIF源数据</param>
        /// <param name="newWidth">新宽</param>
        /// <param name="newHeight">新高</param>
        /// <returns></returns>
        public static byte[] CompressGifByImage(byte[] byteImageIn, int newWidth = 200, int newHeight = 200)
        {
            SixLabors.ImageSharp.Image gif = SixLabors.ImageSharp.Image.Load(byteImageIn);
            var newGif = gif.Clone(x => x.Resize(newWidth, newHeight));
            MemoryStream resMs = new MemoryStream();
            newGif.Save(resMs, new SixLabors.ImageSharp.Formats.Gif.GifEncoder());
            return resMs.ToArray();
        }

        /// <summary>
        /// 对图片进行缩放操作，不包含svg
        /// </summary>
        /// <param name="suffix">图片后缀名</param>
        /// <param name="url">图片url</param>
        /// <param name="certificateFileName">证书名</param>
        /// <param name="certificatePwd">证书密钥</param>
        /// <param name="newWidth">新宽</param>
        /// <param name="newHeight">新高</param>
        /// <returns></returns>
        public static byte[] CompressPicFromUrl(string suffix, string url, string certificateFileName, string certificatePwd, int newWidth = 200, int newHeight = 200)
        {
            //if (url.Substring(0, 5) == "https")
            //{
            //    // 解决WebClient不能通过https下载内容问题
            //    System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            //        delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            //                 System.Security.Cryptography.X509Certificates.X509Chain chain,
            //                 System.Net.Security.SslPolicyErrors sslPolicyErrors)
            //        {
            //            return true; // **** Always accept
            //        };
            //}
            using (WebClient myWebClient = new WebClient())
            {
                var imgData = myWebClient.DownloadData(url);

                if (string.Equals(suffix, ".gif", StringComparison.OrdinalIgnoreCase))
                {
                    return CompressGifByImage(imgData, newWidth, newHeight);
                }
                else
                {
                    return CompressPicByImage(imgData, newWidth, newHeight);
                }
            }
        }

        /// <summary>
        /// 获取svg图像的宽高
        /// </summary>
        /// <param name="node">svg的XML节点数据</param>
        /// <param name="attrName">svg的XML中参数名称</param>
        /// <returns></returns>
        private static string GetAttribute(XmlNode node, string attrName)
        {
            var atts = node.Attributes;
            for (int i = 0; i < atts.Count; i++)
            {
                if (atts[i].Name.Equals(attrName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return atts[i].Value;
                }
            }
            return string.Empty;
            //XmlAttribute tmp = node.OwnerDocument.CreateAttribute(attrName);
            //tmp.Value = value;
            //node.Attributes.Append(tmp);
        }

        /// <summary>
        /// 对svg图片缩放为png图片
        /// </summary>
        /// <param name="url">图片url</param>
        /// <param name="certificateFileName">证书名</param>
        /// <param name="certificatePwd">证书密钥</param>
        /// <param name="newWidth">新宽</param>
        /// <param name="newHeight">新高</param>
        /// <returns></returns>
        public static byte[] CompressSvgFromUrl(string url, string certificateFileName, string certificatePwd, int newWidth = 200, int newHeight = 200)
        {
            //if (url.Substring(0, 5) == "https")
            //{
            //    // 解决WebClient不能通过https下载内容问题
            //    System.Net.ServicePointManager.ServerCertificateValidationCallback +=
            //        delegate (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate,
            //                 System.Security.Cryptography.X509Certificates.X509Chain chain,
            //                 System.Net.Security.SslPolicyErrors sslPolicyErrors)
            //        {
            //            return true; // **** Always accept
            //        };
            //}
            using (WebClient myWebClient = new WebClient())
            {
                var svgData = myWebClient.DownloadData(url);

                return SaveSvgToPng(svgData, newWidth, newHeight);
            }
        }

        /// <summary>
        /// 将svg转为png图片
        /// </summary>
        /// <param name="svgByte">svg图片数据</param>
        /// <param name="newWidth">新宽</param>
        /// <param name="newHeight">新高</param>
        /// <returns></returns>
        public static byte[] SaveSvgToPng(byte[] svgByte, int newWidth = 200, int newHeight = 200)
        {
            using (SKSvg svg = new SKSvg())
            {
                MemoryStream memoryStream = new MemoryStream(svgByte);
                svg.Load(memoryStream);
                using (MemoryStream ms = new MemoryStream())
                {
                    // 新方法
                    SKRect svgSize = svg.Picture.CullRect;
                    float svgMax = Math.Max(svgSize.Width, svgSize.Height);
                    var bitmap = new SKBitmap((int)(svgSize.Width), (int)(svgSize.Height));
                    var canvas = new SKCanvas(bitmap);
                    // 设置抗锯齿为false
                    canvas.DrawPicture(svg.Picture, new SKPaint() { IsAntialias = false });
                    SKImage.FromBitmap(bitmap).Encode(SKEncodedImageFormat.Png, 100).SaveTo(ms);

                    ms.Seek(0, SeekOrigin.Begin);
                    byte[] bytes = new byte[ms.Length];
                    ms.Read(bytes, 0, bytes.Length);
                    ms.Dispose();
                    if (newWidth != 0 && newHeight != 0)
                    {
                        return CompressPicByImage(bytes, newWidth, newHeight);
                    }
                    else
                    {
                        return bytes;
                    }
                }
            }
        }
    }
}
