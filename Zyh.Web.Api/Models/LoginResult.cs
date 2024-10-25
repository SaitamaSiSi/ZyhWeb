namespace Zyh.Web.Api.Models
{
    public class LoginResult
    {
        public string accessToken { get; set; } = string.Empty;
        public string desc { get; set; } = string.Empty;
        public string realName { get; set; } = string.Empty;
        public string userId { get; set; } = string.Empty;
        public string username { get; set; } = string.Empty;
    }
}
