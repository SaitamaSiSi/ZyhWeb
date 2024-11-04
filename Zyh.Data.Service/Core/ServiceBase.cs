//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/10/24 15:33:34</date>
//------------------------------------------------------------------------------

using Zyh.Data.Entity.Core;

namespace Zyh.Data.Service.Core
{
    public class ServiceBase<T> where T : IEntity
    {
        protected string ConnectString = "";
    }
}
