using System;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Sets the path inside App Data, it needs to be read manually later
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyAppDataPathAttribute : Attribute
    {

        /// <summary>
        /// Sets the path inside App Data, it needs to be read manually later
        /// </summary>
        public string Path { get; set; }
    }
}
