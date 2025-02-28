using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using SixLabors.ImageSharp.Processing;

namespace Zyh.Common.AI
{
    public class DetectionResult
    {
        public Rectangle Bounds { get; set; }
        public int ClassId { get; set; }
        public float Confidence { get; set; }
    }

    public class OnnxObjectDetector : BaseAI
    {
        private readonly InferenceSession _session;
        private readonly string[] _labels;
        private readonly string _inputName;
        private readonly int _inputSize;  // 模型输入尺寸（如640）
        private readonly float _confThreshold; // 置信度阈值
        private readonly float _iouThreshold;  // IoU阈值（用于NMS）

        public string[] Labels
        {
            get
            {
                return _labels;
            }
        }

        public OnnxObjectDetector(
            string modelPath,
            string labelPath,
            int inputSize = 1280,
            float confThreshold = 0.5f,
            float iouThreshold = 0.5f)
        {
            _session = new InferenceSession(modelPath);
            _inputName = _session.InputMetadata.Keys.First();
            _labels = File.ReadAllLines(labelPath);
            _inputSize = inputSize;
            _confThreshold = confThreshold;
            _iouThreshold = iouThreshold;
        }

        public OnnxObjectDetector(
            string modelPath,
            string[] labels,
            int inputSize = 1280,
            float confThreshold = 0.5f,
            float iouThreshold = 0.5f)
        {
            _session = new InferenceSession(modelPath);
            _inputName = _session.InputMetadata.Keys.First();
            _labels = labels;
            _inputSize = inputSize;
            _confThreshold = confThreshold;
            _iouThreshold = iouThreshold;
        }

        // 检测并返回结果
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

            // 4. 解析输出（以YOLOv5输出为例）
            var outputTensor = outputs.First().AsTensor<float>();
            var predictions = ParseOutput(outputTensor, scale, pad);

            // 5. 应用非极大值抑制（NMS）
            return ApplyNMS(predictions);
        }

        // 图像预处理（保持长宽比缩放并填充）
        private (Image<Rgb24>, float, (int, int)) PreprocessImage(Image<Rgb24> image)
        {
            // 计算缩放比例
            float scale = Math.Min(
                (float)_inputSize / image.Width,
                (float)_inputSize / image.Height
            );

            // 缩放图像
            var newWidth = (int)(image.Width * scale);
            var newHeight = (int)(image.Height * scale);
            var resizedImage = image.Clone(x => x.Resize(newWidth, newHeight));

            // 填充到输入尺寸
            var padX = _inputSize - newWidth;
            var padY = _inputSize - newHeight;
            resizedImage.Mutate(x => x.Pad(_inputSize, _inputSize, Color.Black));

            return (resizedImage, scale, (padX, padY));
        }

        // 将图像转换为模型输入张量
        private DenseTensor<float> ImageToTensor(Image<Rgb24> image)
        {
            var tensor = new DenseTensor<float>(new[] { 1, 3, _inputSize, _inputSize });
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

        // 解析模型输出（YOLO格式）
        private List<DetectionResult> ParseOutput(Tensor<float> output, float scale, (int padX, int padY) pad)
        {
            var results = new List<DetectionResult>();
            //var outputArray = output.ToArray();
            //int numClasses = _labels.Length; // 动态获取类别数
            //int step = 5 + numClasses;      // 每个检测框的总字段数

            //for (int i = 0; i < output.Dimensions[1]; i++)
            //{
            //    // 获取置信度
            //    float conf = outputArray[i * step + 4];
            //    if (conf < _confThreshold) continue;

            //    // 解析类别概率
            //    var classes = new float[numClasses];
            //    Array.Copy(outputArray, i * step + 5, classes, 0, numClasses);
            //    int classId = Array.IndexOf(classes, classes.Max());
            //    float classScore = classes[classId];
            //    float totalScore = conf * classScore;
            //    if (totalScore < _confThreshold) continue;

            //    // 解析边界框坐标（xywh格式）
            //    float cx = outputArray[i * step + 0];
            //    float cy = outputArray[i * step + 1];
            //    float w = outputArray[i * step + 2];
            //    float h = outputArray[i * step + 3];

            //    // 转换到原始图像坐标（保持原有逻辑）
            //    int x1 = (int)((cx - w / 2 - pad.padX / 2) / scale);
            //    int y1 = (int)((cy - h / 2 - pad.padY / 2) / scale);
            //    int x2 = (int)((cx + w / 2 - pad.padX / 2) / scale);
            //    int y2 = (int)((cy + h / 2 - pad.padY / 2) / scale);

            //    results.Add(new DetectionResult
            //    {
            //        Bounds = new Rectangle(x1, y1, x2 - x1, y2 - y1),
            //        ClassId = classId,
            //        Confidence = totalScore
            //    });
            //}
            return results;
        }

        // 非极大值抑制（NMS）
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

        // 计算IoU（交并比）
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
    }
}
