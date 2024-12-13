//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/18 17:02:00</date>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using System.Xml;

namespace Zyh.Common.Net
{
    public class ReqResult<T> : BaseResult
    {
        public T data { get; set; }

        public ReqResult(T data) : this(data, ResultStatus.Success, 0, string.Empty) { }
        public ReqResult(T data, ResultStatus status) : this(data, status, 0, string.Empty) { }
        public ReqResult(T data, ResultStatus status, int code) : this(data, status, code, string.Empty) { }
        public ReqResult(T data, ResultStatus status, int code, string msg) { this.data = data; base.status = status; base.code = code; message = msg; }

        public static ReqResult<T> Success(T data)
        {
            return new ReqResult<T>(data, ResultStatus.Success, 0, "ok");
        }
        public static ReqResult<T> Failed(T data)
        {
            return new ReqResult<T>(data, ResultStatus.Failed, 0, "error");
        }

        public static ReqResult<T> Success(T data, int code)
        {
            return new ReqResult<T>(data, ResultStatus.Success, code, "ok");
        }
        public static ReqResult<T> Failed(T data, int code)
        {
            return new ReqResult<T>(data, ResultStatus.Failed, code, "error");
        }

        public static ReqResult<T> Success(T data, int code, string msg)
        {
            return new ReqResult<T>(data, ResultStatus.Success, code, msg);
        }
        public static ReqResult<T> Failed(T data, int code, string msg)
        {
            return new ReqResult<T>(data, ResultStatus.Failed, code, msg);
        }

        public string ToJsonString()
        {
            return JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }
    }
}
