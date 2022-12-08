using System;

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
