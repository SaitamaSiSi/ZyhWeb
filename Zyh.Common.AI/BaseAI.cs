using Microsoft.AspNetCore.Http;
using Microsoft.ML.OnnxRuntime;
using System;
using System.Linq;

namespace Zyh.Common.AI
{
    public class BaseAI
    {
        protected string[] _labels; // 标签名
        protected float _confThreshold; // 置信度阈值
        protected float _iouThreshold;  // IoU阈值（用于NMS）
        protected InferenceSession _session;
        protected readonly string[] _modelOutputs;
        protected readonly int _modelOutputDimensions;
        protected string _inputName;
        protected int _inputWidth;
        protected int _inputHeight;

        public string[] Labels
        {
            get
            {
                return _labels;
            }
        }

        public BaseAI(
            string modelPath,
            string[] labels,
            float confThreshold = 0.5f,
            float iouThreshold = 0.5f,
            bool enableCuda = false)
        {
            _labels = labels;
            _confThreshold = confThreshold;
            _iouThreshold = iouThreshold;

            var options = new SessionOptions();
            if (enableCuda)
            {
                options.AppendExecutionProvider_CUDA(); // 需要安装 CUDA 依赖
            }
            // 初始化ONNX推理会话
            _session = new InferenceSession(modelPath, options);

            _inputName = _session.InputNames.First();
            _inputWidth = _session.InputMetadata[_inputName].Dimensions[2];
            _inputHeight = _session.InputMetadata[_inputName].Dimensions[3];
            _modelOutputs = _session.OutputMetadata.Keys.ToArray();
            _modelOutputDimensions = _session.OutputMetadata[_modelOutputs[0]].Dimensions[1];

            // 可以使用自定义标记设置和获取标签
            // _session.ModelMetadata.CustomMetadataMap;
        }
    }
}
