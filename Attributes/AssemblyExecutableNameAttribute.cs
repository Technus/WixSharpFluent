using System;

namespace WixSharp.Fluent.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyExecutableNameAttribute : Attribute
    {
        public string ExecutableName { get; set; }
    }
}
