using System;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Icon path for installer and control panel
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyIconPathAttribute : Attribute
    {
        /// <summary>
        /// The icon path ...\*.ico
        /// </summary>
        public string Path { get; set; }
    }
}
