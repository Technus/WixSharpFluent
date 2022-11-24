using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyStartMenuPathAttribute : Attribute
    {
        /// <summary>
        /// The MSBuild MenuFolderPath variable
        /// </summary>
        public string Path { get; set; }
    }
}
