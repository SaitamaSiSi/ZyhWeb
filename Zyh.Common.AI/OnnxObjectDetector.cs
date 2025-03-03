using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using SixLabors.ImageSharp.Processing;
using System.Threading.Tasks;

namespace Zyh.Common.AI
{
    public class DetectionResult
    {
        public Rectangle Bounds { get; set; }
        public int ClassId { get; set; }
        public float Confidence { get; set; }
    }

    /// <summary>
    /// 目标识别模型助手
    /// </summary>
    public class OnnxObjectDetector : BaseAI
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="modelPath">模型路径</param>
        /// <param name="labels">分类标签</param>
        /// <param name="confThreshold">置信度</param>
        /// <param name="iouThreshold">最大重叠值</param>
        public OnnxObjectDetector(
            string modelPath,
            string[] labels,
            float confThreshold = 0.5f,
            float iouThreshold = 0.5f) : base(modelPath, labels, confThreshold, iouThreshold)
        {
        }

        /// <summary>
        /// 检测并返回结果
        /// </summary>
        /// <param name="imagePath">图像地址</param>
        /// <returns>识别结果集</returns>
        public List<DetectionResult> Detect(string imagePath)
        {
            // 1. 预处理图像
            using var image = Image.Load<Rgb24>(imagePath);
            var (resizedImage, scale, pad) = PreprocessImage(image);

            // 2. 转换为输入张量
            var inputTensor = ImageToTensor(resizedImage);

            // 3. 运行推理
            var inputs = new List<NamedOnnxValue>
            {
                NamedOnnxValue.CreateFromTensor(_inputName, inputTensor)
            };
            using var outputs = _session.Run(inputs);

            // 4. 解析输出（以YOLO输出为例）
            var outputTensor = outputs.First().AsTensor<float>();
            var predictions = ParseOutput(outputTensor, scale, pad, (image.Width, image.Height));

            // 5. 应用非极大值抑制（NMS）
            return ApplyNMS(predictions);
        }

        /// <summary>
        /// 图像预处理（保持长宽比缩放并填充）
        /// </summary>
        /// <param name="image">原图</param>
        /// <returns>缩放图像、缩放倍数、扩充宽高</returns>
        private (Image<Rgb24>, float, (int, int)) PreprocessImage(Image<Rgb24> image)
        {
            // 计算缩放比例
            float scale = Math.Min(
                (float)_inputWidth / image.Width,
                (float)_inputHeight / image.Height
            );

            // 缩放图像
            var newWidth = (int)(image.Width * scale);
            var newHeight = (int)(image.Height * scale);
            var resizedImage = image.Clone(x => x.Resize(newWidth, newHeight));

            // 填充到输入尺寸
            var padX = _inputWidth - newWidth;
            var padY = _inputHeight - newHeight;
            resizedImage.Mutate(x => x.Pad(_inputWidth, _inputHeight, Color.Black));

            return (resizedImage, scale, (padX, padY));
        }

        /// <summary>
        /// 将图像转换为模型输入张量
        /// </summary>
        /// <param name="image">输入图像</param>
        /// <returns>输入张量</returns>
        private DenseTensor<float> ImageToTensor(Image<Rgb24> image)
        {
            var tensor = new DenseTensor<float>(new[] { 1, 3, _inputWidth, _inputHeight });
            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        // 归一化到[0,1]并转换为CHW格式
                        tensor[0, 0, y, x] = row[x].R / 255f; // R通道
                        tensor[0, 1, y, x] = row[x].G / 255f; // G通道
                        tensor[0, 2, y, x] = row[x].B / 255f; // B通道
                    }
                }
            });
            return tensor;
        }

        /// <summary>
        /// 解析模型输出（YOLO格式）[x,y,w,h,s,c]
        /// </summary>
        /// <param name="output">输出结果张量</param>
        /// <param name="scale">缩放倍数</param>
        /// <param name="pad">扩充宽高</param>
        /// <param name="source">原图宽高</param>
        /// <returns>预测识别结果</returns>
        private List<DetectionResult> ParseOutput(Tensor<float> output, float scale, (int padX, int padY) pad, (int w, int h) source)
        {
            var results = new List<DetectionResult>();

            // for each batch
            Parallel.For(0, output.Dimensions[0], i =>
            {
                //divide total length by the elements per prediction
                Parallel.For(0, (int)(output.Length / output.Dimensions[1]), j =>
                {
                    var confidence = output[i, 4, j];

                    //skip low confidence values
                    if (confidence < _confThreshold)
                        return;

                    float xMin = (output[i, 0, j] - output[i, 2, j] / 2 - pad.padX / 2) / scale; // unpad bbox tlx to original
                    float yMin = (output[i, 1, j] - output[i, 3, j] / 2 - pad.padY / 2) / scale; // unpad bbox tly to original
                    float xMax = (output[i, 0, j] + output[i, 2, j] / 2 - pad.padX / 2) / scale; // unpad bbox brx to original
                    float yMax = (output[i, 1, j] + output[i, 3, j] / 2 - pad.padY / 2) / scale; // unpad bbox bry to original

                    xMin = Clamp(xMin, 0, source.w - 0); // clip bbox tlx to boundaries
                    yMin = Clamp(yMin, 0, source.h - 0); // clip bbox tly to boundaries
                    xMax = Clamp(xMax, 0, source.w - 1); // clip bbox brx to boundaries
                    yMax = Clamp(yMax, 0, source.h - 1); // clip bbox bry to boundaries

                    Parallel.For(0, _modelOutputDimensions - 4, delegate (int l)
                    {
                        float classScore = output[new int[3] { i, 4 + l, j }];
                        if (!(classScore < _confThreshold))
                        {
                            results.Add(new DetectionResult()
                            {
                                ClassId = l,
                                Confidence = confidence,
                                Bounds = new Rectangle((int)xMin, (int)yMin, (int)(xMax - xMin), (int)(yMax - yMin))
                            });
                        }
                    });
                });
            });

            return results;
        }

        /// <summary>
        /// 放置识别结果越界
        /// </summary>
        /// <param name="value">当前值</param>
        /// <param name="min">最小值</param>
        /// <param name="max">最大值</param>
        /// <returns></returns>
        private float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        /// <summary>
        /// 非极大值抑制（NMS）
        /// </summary>
        /// <param name="predictions">预测结果集</param>
        /// <returns>去重后结果集</returns>
        private List<DetectionResult> ApplyNMS(List<DetectionResult> predictions)
        {
            var sorted = predictions.OrderByDescending(p => p.Confidence).ToList();
            var results = new List<DetectionResult>();

            while (sorted.Any())
            {
                var current = sorted[0];
                results.Add(current);
                sorted.RemoveAt(0);

                sorted.RemoveAll(p =>
                {
                    var iou = CalculateIoU(current.Bounds, p.Bounds);
                    return iou > _iouThreshold;
                });
            }
            return results;
        }

        /// <summary>
        /// 计算IoU（交并比）
        /// </summary>
        /// <param name="a">框A</param>
        /// <param name="b">框B</param>
        /// <returns>两框IoU</returns>
        private float CalculateIoU(Rectangle a, Rectangle b)
        {
            int x1 = Math.Max(a.Left, b.Left);
            int y1 = Math.Max(a.Top, b.Top);
            int x2 = Math.Min(a.Right, b.Right);
            int y2 = Math.Min(a.Bottom, b.Bottom);

            if (x2 < x1 || y2 < y1) return 0;

            int intersection = (x2 - x1) * (y2 - y1);
            int areaA = a.Width * a.Height;
            int areaB = b.Width * b.Height;
            return (float)intersection / (areaA + areaB - intersection);
        }

        /// <summary>
        /// 绘制预测框
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        public void DrawRectangle(Image<Rgba32> image, int x, int y, int width, int height, Rgba32 color, int thickness = 3)
        {
            // 检查边界
            x = Math.Max(0, x);
            y = Math.Max(0, y);
            width = Math.Min(width, image.Width - x);
            height = Math.Min(height, image.Height - y);

            // 绘制顶部和底部线条
            DrawHorizontalLine(image, x, y, width, color, thickness);
            DrawHorizontalLine(image, x, y + height - 1, width, color, thickness);

            // 绘制左侧和右侧线条
            DrawVerticalLine(image, x, y, height, color, thickness);
            DrawVerticalLine(image, x + width - 1, y, height, color, thickness);
        }

        /// <summary>
        /// 绘制水平线
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="length"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        private void DrawHorizontalLine(Image<Rgba32> image, int x, int y, int length, Rgba32 color, int thickness)
        {
            for (int i = y - thickness; i <= y + thickness; i++)
            {
                for (int j = x; j < x + length; j++)
                {
                    if (i >= 0 && i < image.Height && j >= 0 && j < image.Width)
                    {
                        image[j, i] = color;
                    }
                }
            }
        }

        /// <summary>
        /// 绘制垂直线
        /// </summary>
        /// <param name="image"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="length"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        private void DrawVerticalLine(Image<Rgba32> image, int x, int y, int length, Rgba32 color, int thickness)
        {
            for (int i = y; i < y + length; i++)
            {
                for (int j = x - thickness; j <= x + thickness; j++)
                {
                    if (i >= 0 && i < image.Height && j >= 0 && j < image.Width)
                    {
                        image[j, i] = color;
                    }
                }
            }
        }
    }
}
