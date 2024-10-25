//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/18 17:01:12</date>
//------------------------------------------------------------------------------

namespace Zyh.Common.Net
{
    public class BaseResult
    {
        public int code { get; set; }
        public string message { get; set; } = string.Empty;
        public ResultStatus status { get; set; }

        public BaseResult() { }
        public BaseResult(ResultStatus status) : this(status, 0, string.Empty) { }
        public BaseResult(ResultStatus status, int code) : this(status, code, string.Empty) { }
        public BaseResult(ResultStatus status, int code, string msg) { this.status = status; this.code = code; message = msg; }

        public static BaseResult Success()
        {
            return new BaseResult(ResultStatus.Success);
        }
        public static BaseResult Failed()
        {
            return new BaseResult(ResultStatus.Failed);
        }
    }
}
