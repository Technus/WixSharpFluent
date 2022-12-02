using System;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Sets the path inside program files, it needs to be read manually later
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyProgramFilesPathAttribute : Attribute
    {

        /// <summary>
        /// Sets the path inside program files, it needs to be read manually later
        /// </summary>
        public string Path { get; set; }
    }
}
