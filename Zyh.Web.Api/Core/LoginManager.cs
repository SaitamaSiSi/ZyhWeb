using System.Collections.Concurrent;
using Zyh.Web.Api.Models;

namespace Zyh.Web.Api.Core
{
    public class LoginManager
    {
        private static ConcurrentDictionary<string, LoginClient> OnlineClient { get; set; } = new ConcurrentDictionary<string, LoginClient>();

        public static void Login(LoginClient client)
        {
            OnlineClient.AddOrUpdate(client.userInfo.username, client, (k, v) =>
            {
                v.Clone(client);
                return v;
            });
        }

        public static void Logout(string username)
        {
            OnlineClient.Remove(username, out _);
        }

        public static UserInfo GetInfo(string username)
        {
            OnlineClient.TryGetValue(username, out LoginClient? client);
            return client?.userInfo ?? new UserInfo();
        }

        public static List<string> GetCodes(string username)
        {
            OnlineClient.TryGetValue(username, out LoginClient? client);
            return client?.userCodes ?? new List<string>();
        }
    }
}
