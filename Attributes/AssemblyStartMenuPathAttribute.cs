using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Sets the path inside start menu
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyStartMenuPathAttribute : Attribute
    {
        /// <summary>
        /// Sets the path inside start menu
        /// </summary>
        public string Path { get; set; }
    }
}
