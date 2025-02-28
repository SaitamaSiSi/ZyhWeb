using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Numerics;
using Zyh.Common.Dto;

namespace Zyh.Common.Drawing
{
    public class BezierUtils
    {
        /// <summary>
        /// 贝塞尔转图
        /// </summary>
        /// <param name="bPath"></param>
        /// <returns></returns>
        public static byte[] BezierToImage(BezierPath bPath)
        {
            // 起点、终点如果定位为球体左上角，则需要修正
            bPath.Source.X += bPath.LineWidth / 2;
            bPath.Source.Y += bPath.LineWidth / 2;
            bPath.Target.X += bPath.LineWidth / 2;
            bPath.Target.Y += bPath.LineWidth / 2;

            // 二次贝塞尔曲线分段
            var bezierLength = GetBezierDistance(bPath.Source, bPath.Vertices, bPath.Target);
            var arrowNum = (int)(8 - bezierLength / 40);
            bPath.SplitNum = (arrowNum >= 0 && arrowNum <= 8) ? arrowNum + 2 : 2;
            // 分为10段+箭头段数
            bPath = OptimizeToMoreBezier(bPath, 10 + bPath.SplitNum.Value);

            if (bPath.Shape == BezierType.曲线)
            {
                // 每三个点是一段贝塞尔曲线，控制点的颜色是该路段颜色
                // 根据colorNodes集合给每段赋颜色，总长为totalDistance，不足使用默认值，超过则丢弃。
                List<RealPosition> colorNodes = new List<RealPosition>();
                int totalDistance = 0;
                bPath.Vertices = SetBezierColor(bPath.Vertices, colorNodes, totalDistance);
            }

            return SKDrawPic(bPath);
        }

        /// <summary>
        /// 绘制图
        /// </summary>
        /// <param name="bPath"></param>
        /// <param name="fotmat"></param>
        /// <param name="qulity"></param>
        /// <returns></returns>
        protected static byte[] SKDrawPic(BezierPath bPath, SkiaSharp.SKEncodedImageFormat fotmat = SkiaSharp.SKEncodedImageFormat.Png, int qulity = 100)
        {
            // 初始化图像
            var skDrawHelper = new SkiaSharpHelper(500, 500);
            SkiaSharp.SKColor.TryParse("#0000FF", out var bgColor);
            skDrawHelper.Init(bgColor);

            // 绘制
            if (bPath.Shape == BezierType.曲线)
            {
                skDrawHelper.DrawRoad(bPath.Vertices, bPath.LineWidth, bPath.Shape, arrowNum: bPath.SplitNum.HasValue ? bPath.SplitNum.Value : 2);

            }
            else if (bPath.Shape == BezierType.辅助线)
            {
                skDrawHelper.DrawRoad(bPath.Vertices, bPath.LineWidth, bPath.Shape);
            }

            skDrawHelper.SaveToImage($@"C:/Users/Administrator/Desktop/SkiaSharp_{DateTime.Now.ToString("HHmmss")}.png");
            return skDrawHelper.SaveToByte(fotmat, qulity);
        }

        /// <summary>
        /// 分段生成贝塞尔路径
        /// </summary>
        /// <param name="path"></param>
        /// <param name="splitNum"></param>
        /// <returns></returns>
        protected static BezierPath OptimizeToMoreBezier(BezierPath path, int splitNum)
        {
            if (path == null)
            {
                return new BezierPath();
            }

            BezierPath respath = JsonConvert.DeserializeObject<BezierPath>(JsonConvert.SerializeObject(path));

            if (path.Vertices != null && respath != null && respath.Vertices != null)
            {
                respath.Vertices.Clear();

                if (path.Vertices.Count == 0)
                {
                    respath.Vertices.AddRange(DivideBezier(
                        path.Source,
                        new BezierPosition() { X = (path.Source.X + path.Target.X) / 2, Y = (path.Source.Y + path.Target.Y) / 2 },
                        path.Target,
                        splitNum));
                }
                else if (path.Vertices.Count == 1)
                {
                    respath.Vertices.AddRange(DivideBezier(
                        path.Source,
                        path.Vertices[0],
                        path.Target,
                        splitNum));
                }
                else
                {
                    var allDistance = GetBezierDistance(path.Source, path.Vertices, path.Target);
                    for (int i = 0; i < path.Vertices.Count; i++)
                    {
                        if (i == 0)
                        {
                            var distance = GetCurveLenght(
                        new Vector2() { X = path.Source.X, Y = path.Source.Y },
                        new Vector2() { X = path.Vertices[i].X, Y = path.Vertices[i].Y },
                        new Vector2() { X = (path.Vertices[i].X + path.Vertices[i + 1].X) / 2, Y = (path.Vertices[i].Y + path.Vertices[i + 1].Y) / 2 }
                        );
                            int singleSplitNum = (int)(distance / allDistance * 100);
                            respath.Vertices.AddRange(DivideBezier(
                            path.Source,
                            path.Vertices[i],
                            new BezierPosition() { X = (path.Vertices[i].X + path.Vertices[i + 1].X) / 2, Y = (path.Vertices[i].Y + path.Vertices[i + 1].Y) / 2 },
                            singleSplitNum));
                        }
                        else if (i == path.Vertices.Count - 1)
                        {
                            var distance = GetCurveLenght(
                        new Vector2() { X = (path.Vertices[i - 1].X + path.Vertices[i].X) / 2, Y = (path.Vertices[i - 1].Y + path.Vertices[i].Y) / 2 },
                        new Vector2() { X = path.Vertices[i].X, Y = path.Vertices[i].Y },
                        new Vector2() { X = path.Target.X, Y = path.Target.Y }
                        );
                            int singleSplitNum = (int)(distance / allDistance * 100);
                            respath.Vertices.AddRange(DivideBezier(
                                new BezierPosition() { X = (path.Vertices[i - 1].X + path.Vertices[i].X) / 2, Y = (path.Vertices[i - 1].Y + path.Vertices[i].Y) / 2 },
                                path.Vertices[i],
                                new BezierPosition() { X = path.Target.X, Y = path.Target.Y },
                                singleSplitNum));
                        }
                        else
                        {
                            var distance = GetCurveLenght(
                        new Vector2() { X = (path.Vertices[i - 1].X + path.Vertices[i].X) / 2, Y = (path.Vertices[i - 1].Y + path.Vertices[i].Y) / 2 },
                        new Vector2() { X = path.Vertices[i].X, Y = path.Vertices[i].Y },
                        new Vector2() { X = (path.Vertices[i].X + path.Vertices[i + 1].X) / 2, Y = (path.Vertices[i].Y + path.Vertices[i + 1].Y) / 2 }
                        );
                            int singleSplitNum = (int)(distance / allDistance * 100);
                            respath.Vertices.AddRange(DivideBezier(
                                new BezierPosition() { X = (path.Vertices[i - 1].X + path.Vertices[i].X) / 2, Y = (path.Vertices[i - 1].Y + path.Vertices[i].Y) / 2 },
                                path.Vertices[i],
                                new BezierPosition() { X = (path.Vertices[i].X + path.Vertices[i + 1].X) / 2, Y = (path.Vertices[i].Y + path.Vertices[i + 1].Y) / 2 },
                                singleSplitNum));
                        }
                    }
                }
                // 将最后的节点改为终点
                respath.Vertices[^1] = path.Target;
            }

            return respath ?? new BezierPath();
        }

        /// <summary>
        /// 使用此函数，必须要先使用分段函数
        /// </summary>
        /// <param name="BezierPositions"></param>
        /// <param name="colorNodes"></param>
        /// <param name="totalDistance"></param>
        /// <returns></returns>
        protected static List<BezierPosition> SetBezierColor(List<BezierPosition> BezierPositions, List<RealPosition> colorNodes, double totalDistance)
        {
            int index = 0;

            if (BezierPositions == null)
            {
                return new List<BezierPosition>();
            }

            if (colorNodes != null && colorNodes.Count >= 2 && totalDistance != 0)
            {
                for (int i = 1; i < colorNodes.Count; i++)
                {
                    var percentage = (int)(Math.Floor(colorNodes[i].Distance / totalDistance * 100));
                    int pos = 0;
                    for (int j = index; j < BezierPositions.Count; j++, index++)
                    {
                        if (BezierPositions[j].IsControl)
                        {
                            // 根据状态对应颜色
                            BezierPositions[j].Status = colorNodes[i].Status == 0 ? "#00FF00" : "#0000FF";
                            pos++;
                        }
                        if (pos == percentage)
                        {
                            index++;
                            break;
                        }
                    }
                }

                // 补全箭头颜色
                var lastColor = colorNodes[^1].Status == 0 ? "#00FF00" : "#0000FF";
                for (int i = index; i < BezierPositions.Count; i++)
                {
                    if (BezierPositions[i].IsControl)
                    {
                        BezierPositions[i].Status = lastColor;
                    }
                }
            }
            else
            {
                for (int i = index; i < BezierPositions.Count; i++)
                {
                    if (BezierPositions[i].IsControl)
                    {
                        // 无数据时为绿色
                        BezierPositions[i].Status = "#00FF00";
                    }
                }
            }

            return BezierPositions;
        }

        /// <summary>
        /// 将贝塞尔曲线分段
        /// </summary>
        /// <param name="start">起点</param>
        /// <param name="control">控制点</param>
        /// <param name="end">终点</param>
        /// <param name="num">段数</param>
        /// <returns></returns>
        protected static List<BezierPosition> DivideBezier(BezierPosition start, BezierPosition control, BezierPosition end, int num)
        {
            var res = new List<BezierPosition>();
            var tmp = new List<BezierPosition>();

            if (num > 1)
            {
                var addNum = num * 2;
                var xStartDistance = control.X - start.X;
                var yStartDistance = control.Y - start.Y;
                var xEndDistance = end.X - control.X;
                var yEndDistance = end.Y - control.Y;

                //生成偶数段
                for (int i = 1; i < addNum; i++)
                {
                    var newNodeStartX = start.X + xStartDistance / addNum * i;
                    var newNodeStartY = start.Y + yStartDistance / addNum * i;
                    var newNodeEndX = control.X + xEndDistance / addNum * i;
                    var newNodeEndY = control.Y + yEndDistance / addNum * i;
                    var newNode = new BezierPosition()
                    {
                        X = newNodeStartX + (newNodeEndX - newNodeStartX) / addNum * i,
                        Y = newNodeStartY + (newNodeEndY - newNodeStartY) / addNum * i
                    };
                    tmp.Add(newNode);
                }

                //计算中间控制点
                for (int i = 0; i < num; i++)
                {
                    if (num == 1)
                    {
                        res.Add(start);
                        res.Add(new BezierPosition()
                        {
                            X = tmp[i * 2].X * 2 - start.X / 2 - end.X / 2,
                            Y = tmp[i * 2].Y * 2 - start.Y / 2 - end.Y / 2,
                            IsControl = true
                        });
                        res.Add(end);
                    }
                    else
                    {
                        if (i == 0)
                        {
                            res.Add(start);
                            res.Add(new BezierPosition()
                            {
                                X = tmp[i * 2].X * 2 - start.X / 2 - tmp[i * 2 + 1].X / 2,
                                Y = tmp[i * 2].Y * 2 - start.Y / 2 - tmp[i * 2 + 1].Y / 2,
                                IsControl = true
                            });
                            res.Add(tmp[i * 2 + 1]);
                        }
                        else if (i == num - 1)
                        {
                            res.Add(tmp[i * 2 - 1]);
                            res.Add(new BezierPosition()
                            {
                                X = tmp[i * 2].X * 2 - tmp[i * 2 - 1].X / 2 - end.X / 2,
                                Y = tmp[i * 2].Y * 2 - tmp[i * 2 - 1].Y / 2 - end.Y / 2,
                                IsControl = true
                            });
                            res.Add(end);
                        }
                        else
                        {
                            res.Add(tmp[i * 2 - 1]);
                            res.Add(new BezierPosition()
                            {
                                X = tmp[i * 2].X * 2 - tmp[i * 2 - 1].X / 2 - tmp[i * 2 + 1].X / 2,
                                Y = tmp[i * 2].Y * 2 - tmp[i * 2 - 1].Y / 2 - tmp[i * 2 + 1].Y / 2,
                                IsControl = true
                            });
                            res.Add(tmp[i * 2 + 1]);
                        }
                    }
                }
            }
            else
            {
                res.Add(start);
                control.IsControl = true;
                res.Add(control);
                res.Add(end);
            }

            return res;
        }

        /// <summary>
        /// 获取贝塞尔边总长(在未分段之前,即vetcices中全是控制点时,以及中间节点大于等于2时使用)
        /// </summary>
        /// <param name="start"></param>
        /// <param name="vertices"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        protected static double GetBezierDistance(BezierPosition start, List<BezierPosition> vertices, BezierPosition end)
        {
            double distance = 0;

            if (vertices.Count >= 2)
            {
                for (int i = 0; i < vertices.Count; i++)
                {
                    if (i == 0)
                    {
                        distance += GetCurveLenght(
                        new Vector2() { X = start.X, Y = start.Y },
                        new Vector2() { X = vertices[i].X, Y = vertices[i].Y },
                        new Vector2() { X = (vertices[i].X + vertices[i + 1].X) / 2, Y = (vertices[i].Y + vertices[i + 1].Y) / 2 }
                        );
                    }
                    else if (i == vertices.Count - 1)
                    {
                        distance += GetCurveLenght(
                        new Vector2() { X = (vertices[i - 1].X + vertices[i].X) / 2, Y = (vertices[i - 1].Y + vertices[i].Y) / 2 },
                        new Vector2() { X = vertices[i].X, Y = vertices[i].Y },
                        new Vector2() { X = end.X, Y = end.Y }
                        );
                    }
                    else
                    {
                        distance += GetCurveLenght(
                        new Vector2() { X = (vertices[i - 1].X + vertices[i].X) / 2, Y = (vertices[i - 1].Y + vertices[i].Y) / 2 },
                        new Vector2() { X = vertices[i].X, Y = vertices[i].Y },
                        new Vector2() { X = (vertices[i].X + vertices[i + 1].X) / 2, Y = (vertices[i].Y + vertices[i + 1].Y) / 2 }
                        );
                    }
                }
            }
            else if (vertices.Count == 1)
            {
                distance += GetCurveLenght(
                        new Vector2() { X = start.X, Y = start.Y },
                        new Vector2() { X = vertices[0].X, Y = vertices[0].Y },
                        new Vector2() { X = end.X, Y = end.Y }
                        );
            }

            return distance;
        }

        /// <summary>
        /// 求二阶贝塞尔曲线长度
        /// </summary>
        /// <param name="vecA"></param>
        /// <param name="vecB"></param>
        /// <param name="vecC"></param>
        protected static double GetCurveLenght(Vector2 vecA, Vector2 vecB, Vector2 vecC)
        {
            double curveLenght = CalculateCalculusValue(0.0f, vecA, vecB, vecC) - CalculateCalculusValue(1.0f, vecA, vecB, vecC);

            if (Double.IsNaN(curveLenght))
            {
                curveLenght = Math.Abs(Math.Sqrt(Math.Pow(vecC.X - vecA.X, 2) + Math.Pow(vecC.Y - vecA.Y, 2)));
            }
            else
            {
                curveLenght = Math.Abs(curveLenght);
            }

            return curveLenght;
        }

        protected static double CalculateCalculusValue(float t, Vector2 vecA, Vector2 vecB, Vector2 vecC)
        {
            Vector2 va = vecA + vecC - 2 * vecB;
            Vector2 vb = 2 * (vecB - vecA);
            double A = 4 * (va.X * va.X + va.Y * va.Y);
            double B = 4 * (va.X * vb.X + va.Y * vb.Y);
            double C = vb.X * vb.X + vb.Y * vb.Y;

            //计算多项式的某些项的值，减少相同运算次数
            double sa = Math.Sqrt(A);
            double satb = 2 * A * t + B;
            double satbtc = 2 * sa * Math.Sqrt(A * t * t + B * t + C);
            double sa23 = 8 * A * sa;

            double result = (satbtc * satb - (B * B - 4 * A * C) * Math.Log(satbtc + satb)) / sa23;
            return result;
        }

    }
}
