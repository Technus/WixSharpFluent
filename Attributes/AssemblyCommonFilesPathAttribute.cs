using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Sets the path inside Common files, it needs to be read manually later
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyCommonFilesPathAttribute : Attribute
    {

        /// <summary>
        /// Sets the path inside Common files, it needs to be read manually later
        /// </summary>
        public string Path { get; set; }
    }
}
