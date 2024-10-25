namespace Zyh.Web.Api.Models
{
    public class LoginClient
    {
        public UserInfo userInfo { get; set; } = new UserInfo();
        public List<string> userCodes { get; set; } = new List<string>();

        public void Clone(LoginClient source)
        {
            this.userInfo.Clone(source.userInfo);
            this.userCodes = source.userCodes;
        }
    }
}
