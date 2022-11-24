using System;
using System.Linq;

namespace WixSharp.Fluent.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyIconPathAttribute : Attribute
    {
        public string Path { get; set; }
    }
}
