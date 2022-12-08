using System;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// The installer file name
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyExecutableNameAttribute : Attribute
    {
        /// <summary>
        /// Sets the installer file name (without the extension, as it is automatic)
        /// </summary>
        public string ExecutableName { get; set; }
    }
}
