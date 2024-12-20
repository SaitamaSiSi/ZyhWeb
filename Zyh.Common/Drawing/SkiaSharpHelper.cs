//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/12/20 9:41:40</date>
//------------------------------------------------------------------------------

using SkiaSharp;
using System;
using System.Collections.Generic;
using System.IO;
using Zyh.Common.Dto;

namespace Zyh.Common.Drawing
{
    public class SkiaSharpHelper
    {
        public SKImageInfo skInfo { get; private set; }
        public SKBitmap skBitmap { get; private set; }
        public SKCanvas skCanvas { get; private set; }

        public SkiaSharpHelper(int width, int height)
        {
            skInfo = new SKImageInfo(width, height);
            skBitmap = new SKBitmap(skInfo);
            skCanvas = new SKCanvas(skBitmap);
            skCanvas.DrawColor(SKColors.Blue);
        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="color">整个图片的背景底色</param>
        public void Init(SKColor color)
        {
            skCanvas.DrawColor(color);
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <param name="absolutePath">图片绝对路径</param>
        /// <param name="fotmat">图片保存格式</param>
        /// <param name="qulity">图片质量</param>
        public void SaveToImage(string absolutePath, SKEncodedImageFormat fotmat = SKEncodedImageFormat.Png, int qulity = 100)
        {
            var path = Path.GetDirectoryName(absolutePath);
            if (!Directory.Exists(path))
            {
                //创建文件夹
                Directory.CreateDirectory(path);
            }

            var data = SKImage.FromBitmap(skBitmap).Encode(fotmat, qulity);
            using (FileStream fs = File.Create(absolutePath))
            {
                data.SaveTo(fs);
            }
        }

        /// <summary>
        /// 保存图片
        /// </summary>
        /// <returns></returns>
        public byte[] SaveToByte(SKEncodedImageFormat fotmat = SKEncodedImageFormat.Png, int qulity = 100)
        {
            if (skBitmap != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    SKImage.FromBitmap(skBitmap).Encode(fotmat, qulity).SaveTo(ms);
                    return ms.ToArray();
                }
            }
            return new byte[0];
        }

        /// <summary>
        /// 绘制贝塞尔路径信息，要求，贝塞尔点个数必须是3倍数的点否则绘制起点到终点的直线，颜色取中间控制点的颜色
        /// </summary>
        /// <param name="posList">经过分段函数得到的贝塞尔点集合</param>
        /// <param name="strokeWidth">宽度</param>
        /// <param name="isStright">是否只有两点一线</param>
        public void DrawRoad(List<BezierPosition> posList, float strokeWidth, BezierType way, bool arrow = true, string assistColor = "#00FF00", int arrowNum = 2)
        {
            skCanvas.Save();
            using (var paint = new SKPaint())
            {
                // 抗锯齿不能少
                paint.IsAntialias = false;
                // 道路宽度
                paint.StrokeWidth = strokeWidth;
                // 设置空心，只绘制边框
                paint.Style = SKPaintStyle.Stroke;
                // 图像宽度
                var canvasWidth = skBitmap.Width;
                // 图像高度
                var canvasHeight = skBitmap.Height;
                var arrowList = new List<SKPoint>();
                var arrowNeedNum = arrowNum * 3;
                // 曲线
                if (way == BezierType.曲线 && posList.Count >= arrowNeedNum && posList.Count % 3 == 0)
                {
                    for (int i = posList.Count - arrowNeedNum - 1; i >= 0; i -= 3)
                    {
                        SKPath skPath = new SKPath();
                        SKColor.TryParse(posList[i - 1].Status, out var skColor);
                        paint.Color = skColor;

                        skPath.MoveTo(posList[0].X, posList[0].Y);
                        for (int j = 0; j < i; j += 3)
                        {
                            skPath.QuadTo(
                                    new SKPoint() { X = posList[j + 1].X, Y = posList[j + 1].Y },
                                    new SKPoint() { X = posList[j + 2].X, Y = posList[j + 2].Y }
                                    );
                        }
                        skCanvas.DrawPath(skPath, paint);
                    }
                    if (arrow)
                    {
                        arrowList = CaculateArrow(new SKPoint() { X = posList[^1].X, Y = posList[^1].Y }, new SKPoint() { X = posList[posList.Count - arrowNeedNum].X, Y = posList[posList.Count - arrowNeedNum].Y }, strokeWidth);
                    }
                }
                // 辅助线
                else if (way == BezierType.辅助线 && posList.Count >= arrowNeedNum && posList.Count % 3 == 0)
                {
                    SKColor.TryParse(assistColor, out var skColor);
                    paint.Color = skColor;
                    for (int i = posList.Count - arrowNeedNum - 1; i >= 0; i -= 3)
                    {
                        SKPath skPath = new SKPath();

                        skPath.MoveTo(posList[0].X, posList[0].Y);
                        for (int j = 0; j < i; j += 3)
                        {
                            skPath.QuadTo(
                                    new SKPoint() { X = posList[j + 1].X, Y = posList[j + 1].Y },
                                    new SKPoint() { X = posList[j + 2].X, Y = posList[j + 2].Y }
                                    );
                        }
                        skCanvas.DrawPath(skPath, paint);
                    }
                    if (arrow)
                    {
                        arrowList = CaculateArrow(new SKPoint() { X = posList[^1].X, Y = posList[^1].Y }, new SKPoint() { X = posList[posList.Count - arrowNeedNum].X, Y = posList[posList.Count - arrowNeedNum].Y }, strokeWidth);
                    }
                }
                if (arrowList != null && arrowList.Count == 3)
                {
                    SKColor.TryParse(assistColor, out var skColor);
                    if (way == BezierType.曲线)
                    {
                        SKColor.TryParse(posList[posList.Count - 2].Status, out skColor);
                    }
                    var vect = SKVertices.CreateCopy(SKVertexMode.Triangles, arrowList.ToArray(), null);
                    skCanvas.DrawVertices(vect, SKBlendMode.Clear, new SKPaint() { Color = skColor, IsAntialias = false });
                }
            }
            skCanvas.Restore();
            skCanvas.Flush();
        }

        /// <summary>
        /// 通过传入三角形顶点坐标以及底边中点坐标及底边宽度，求出另外两点坐标
        /// </summary>
        /// <param name="top"></param>
        /// <param name="bottom"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        protected List<SKPoint> CaculateArrow(SKPoint top, SKPoint bottom, float width)
        {
            var res = new List<SKPoint>();

            var lineA = -(top.X - bottom.X) / (int)(top.Y - bottom.Y); // 底边线的斜率
            var lineB = (lineA * bottom.X - bottom.Y) * -1; // 底边线的偏移量

            if (!float.IsInfinity(lineA) && !float.IsInfinity(lineB))
            {
                var a = 1 + Math.Pow(lineA, 2);
                var b = 2 * (lineA * lineB - bottom.X - lineA * bottom.Y);
                var c = Math.Pow(bottom.X, 2) + Math.Pow(bottom.Y, 2) + Math.Pow(lineB, 2) - 2 * lineB * bottom.Y - Math.Pow((width / 2), 2);

                var xa = (-1 * b + Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);
                var xb = (-1 * b - Math.Sqrt(Math.Pow(b, 2) - 4 * a * c)) / (2 * a);

                var ya = lineA * xa + lineB;
                var yb = lineA * xb + lineB;

                res.Add(top);
                res.Add(new SKPoint() { X = (float)xa, Y = (float)ya });
                res.Add(new SKPoint() { X = (float)xb, Y = (float)yb });
            }
            else
            {

                res.Add(top);
                res.Add(new SKPoint() { X = (float)bottom.X, Y = (float)(bottom.Y + width / 2) });
                res.Add(new SKPoint() { X = (float)bottom.X, Y = (float)(bottom.Y - width / 2) });
            }

            return res;
        }

    }
}
