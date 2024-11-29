using System.Collections.Concurrent;
using Zyh.Web.Api.Models;

namespace Zyh.Web.Api.Mysql
{
    public class TempUsers
    {
        public static ConcurrentDictionary<string, UserInfo> SignInClients = new ConcurrentDictionary<string, UserInfo>();
    }
}
