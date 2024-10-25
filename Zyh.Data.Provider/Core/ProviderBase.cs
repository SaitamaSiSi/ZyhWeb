//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 14:48:58</date>
//------------------------------------------------------------------------------

using Zyh.Data.Entity.Core;

namespace Zyh.Data.Provider.Core
{
    public class ProviderBase<T> where T : IEntity
    {
        private string _connectString = "HOST=192.168.100.188;PORT=5432;DATABASE=db_led;USER ID=test;PASSWORD=test@123;No Reset On Close=true";

        protected string ConnectString
        {
            get { return _connectString; }
            set { _connectString = value; }
        }
    }
}
