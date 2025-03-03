using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using System;
using System.Collections.Generic;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Linq;

namespace Zyh.Common.AI
{
    /// <summary>
    /// 分类识别模型助手
    /// </summary>
    public class OnnxImageClassifier : BaseAI
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="modelPath">模型路径</param>
        /// <param name="labels">分类标签</param>
        /// <param name="enableCuda">是否开启Cuda加速</param>
        public OnnxImageClassifier(
            string modelPath,
            string[] labels,
            bool enableCuda = false) : base(modelPath, labels, 0, 0, enableCuda)
        {
        }

        /// <summary>
        /// 图像分类
        /// </summary>
        /// <param name="imagePath">图像路径</param>
        /// <returns>分类结果</returns>
        public Dictionary<string, float> ClassifyImage(string imagePath)
        {
            // 预处理图像
            using var image = Image.Load<Rgb24>(imagePath);
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
                NamedOnnxValue.CreateFromTensor(_inputName, inputTensor) // "input"需替换为模型实际输入名称
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
