using System;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Sets the path to content
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblySourcePathAttribute : Attribute
    {
        /// <summary>
        /// The MSBuild SourcePath variable
        /// </summary>
        public string Path { get; set; }
    }
}
