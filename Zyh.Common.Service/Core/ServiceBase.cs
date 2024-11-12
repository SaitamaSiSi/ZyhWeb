//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/11/12 15:31:25</date>
//------------------------------------------------------------------------------

using Zyh.Common.Entity.Core;

namespace Zyh.Common.Service.Core
{
    public class ServiceBase<T> where T : IEntity
    {
        protected string ConnectString = "DefaultConnectionString";
    }
}
