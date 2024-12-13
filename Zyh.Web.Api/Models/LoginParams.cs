﻿namespace Zyh.Web.Api.Models
{
    public class LoginParams
    {
        public bool withCredentials { get; set; }
        public string username { get; set; } = string.Empty;
        public string password { get; set; } = string.Empty;
        public string accessToken { get; set; } = string.Empty;
        public string refreshToken { get; set; } = string.Empty;
    }
}
