using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyProgramFilesPathAttribute : Attribute
    {

        /// <summary>
        /// The MSBuild InstallationPath variable
        /// </summary>
        public string Path { get; set; }
    }
}
