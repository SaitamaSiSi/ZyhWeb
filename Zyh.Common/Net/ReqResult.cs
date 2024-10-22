//------------------------------------------------------------------------------
// <copyright file="HttpResult.cs" company="CQ ULIT Co., Ltd.">
//    Copyright (c) 2024, Chongqing Youliang Science & Technology Co., Ltd. All rights reserved.
// </copyright>
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
            return new ReqResult<T>(data, ResultStatus.Success);
        }
        public static ReqResult<T> Failed(T data)
        {
            return new ReqResult<T>(data, ResultStatus.Failed);
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
