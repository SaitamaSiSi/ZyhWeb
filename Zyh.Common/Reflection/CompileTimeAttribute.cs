//------------------------------------------------------------------------------
// <author>Zhuo YuHan</author>
// <email>1719700768@qq.com</email>
// <date>2024/09/19 10:50:44</date>
//------------------------------------------------------------------------------

using System;
using System.Globalization;

namespace Zyh.Common.Reflection
{
    /// <summary>
    /// 程序集编译时间Attribute
    /// 需要获取程序集的编译时间时, 在*.csproj文件中添加以下代码:
    ///   <ItemGroup>
    ///     <AssemblyAttribute Include = "Ebos.Common.Reflection.CompileTimeAttribute" >
    ///         <_Parameter1>$([System.DateTime]::Now.ToString("yyyyMMddHHmmss"))</_Parameter1>
    ///     </AssemblyAttribute>
    ///   </ItemGroup>
    /// </summary>
    /// <example>
    /// </example>
    [Obsolete("Deprecated, Suggest using System.Reflection.AssemblyMetadataAttribute")]
    [AttributeUsage(AttributeTargets.Assembly)]
    public class CompileTimeAttribute : Attribute
    {
        public CompileTimeAttribute(string value)
        {
            this.Time = DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.CurrentCulture, DateTimeStyles.None);
        }

        public DateTime Time { get; }

    }
}
