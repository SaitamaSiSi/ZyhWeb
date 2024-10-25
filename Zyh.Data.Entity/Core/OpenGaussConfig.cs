//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 14:53:27</date>
//------------------------------------------------------------------------------

namespace Zyh.Data.Entity.Core
{
    public class OpenGaussConfig
    {
        public string Host { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 5432;
        public string Database { get; set; } = "postgres";
        public string UserId { get; set; } = string.Empty;
        public string UserPwd { get; set; } = string.Empty;

        public string GetConnectString()
        {
            if (string.IsNullOrEmpty(this.Host) || this.Port < 0 || this.Port > 65535 ||
                string.IsNullOrEmpty(this.Database) || string.IsNullOrEmpty(this.UserId)
                || string.IsNullOrEmpty(this.UserPwd))
            {
                return string.Empty;
            }
            return $"HOST={this.Host};PORT={this.Port};DATABASE={this.Database};USER ID={this.UserId};PASSWORD={this.UserPwd};No Reset On Close=true";
        }
    }
}
