namespace Zyh.Web.Api.Models
{
    public class UserInfo
    {
        /// <summary>
        /// 头像
        /// </summary>
        public string avatar { get; set; } = string.Empty;
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string realName { get; set; } = string.Empty;
        /// <summary>
        /// 用户角色
        /// </summary>
        public List<string> roles { get; set; } = new List<string>();
        /// <summary>
        /// 用户id
        /// </summary>
        public string userId { get; set; } = string.Empty;
        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; } = string.Empty;

        public void Clone(UserInfo source)
        {
            this.avatar = source.avatar;
            this.realName = source.realName;
            this.roles = source.roles;
            this.userId = source.userId;
            this.username = source.username;
        }
    }
}
