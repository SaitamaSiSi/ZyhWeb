using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Linq;

namespace Zyh.Common.AI
{
    public class OnnxImageClassifier : BaseAI
    {
        private readonly InferenceSession _session;
        private readonly string[] _labels;

        public OnnxImageClassifier(string modelPath, string labelPath)
        {
            // 初始化ONNX推理会话
            _session = new InferenceSession(modelPath);
            // 加载标签
            _labels = File.ReadAllLines(labelPath);
        }

        public OnnxImageClassifier(string modelPath, string[] labels)
        {
            // 初始化ONNX推理会话
            _session = new InferenceSession(modelPath);
            // 加载标签
            _labels = labels;
        }

        public OnnxImageClassifier(string modelPath, bool enableCuda = false)
        {
            var options = new SessionOptions();
            options.AppendExecutionProvider_CUDA(); // 需要安装 CUDA 依赖
            // 初始化ONNX推理会话
            _session = enableCuda ? new InferenceSession(modelPath, options) : new InferenceSession(modelPath);
            // 加载标签
            _labels = new string[0];
        }

        public Dictionary<string, float> ClassifyImage(string imagePath)
        {
            // 预处理图像
            using var image = Image.Load<Rgb24>(imagePath);
            string inputName = _session.InputMetadata.Keys.First();
            int height = _session.InputMetadata[inputName].Dimensions[2];
            int width = _session.InputMetadata[inputName].Dimensions[3];
            // _session.ModelMetadata.CustomMetadataMap;
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(224, 224),
                Mode = ResizeMode.Stretch // 根据模型要求调整
            }));

            // 转换为张量并归一化（ImageNet标准化参数）
            float[] input = new float[1 * 3 * 224 * 224];
            var mean = new[] { 0.485f, 0.456f, 0.406f };
            var std = new[] { 0.229f, 0.224f, 0.225f };

            image.ProcessPixelRows(accessor =>
            {
                for (int y = 0; y < accessor.Height; y++)
                {
                    Span<Rgb24> row = accessor.GetRowSpan(y);
                    for (int x = 0; x < row.Length; x++)
                    {
                        // 获取像素值并标准化
                        input[0 * 224 * 224 + y * 224 + x] = (row[x].R / 255f - mean[0]) / std[0]; // R
                        input[1 * 224 * 224 + y * 224 + x] = (row[x].G / 255f - mean[1]) / std[1]; // G
                        input[2 * 224 * 224 + y * 224 + x] = (row[x].B / 255f - mean[2]) / std[2]; // B
                    }
                }
            });

            // 创建输入Tensor
            var inputTensor = new DenseTensor<float>(input, new[] { 1, 3, 224, 224 });
            var inputs = new List<NamedOnnxValue>{
                NamedOnnxValue.CreateFromTensor(inputName, inputTensor) // "input"需替换为模型实际输入名称
            };

            // 运行推理
            using var results = _session.Run(inputs);
            var output = results.First().AsTensor<float>();

            List<double> predict_cla = new List<double>();
            foreach (var item in output)
            {
                predict_cla.Add(1.0 / (1.0 + Math.Exp(-item)));
            }
            var maxConfidence = predict_cla.Max();
            var index = predict_cla.IndexOf(maxConfidence);
            var label = _labels[index];
            var score = (float)maxConfidence;
            return new Dictionary<string, float>
            {
                { label, score }
            };
        }
    }
}
