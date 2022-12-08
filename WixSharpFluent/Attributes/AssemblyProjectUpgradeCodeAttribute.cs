using System;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Attribute to store the product and bundle upgrade codes
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyProjectUpgradeCodeAttribute : Attribute
    {
        /// <summary>
        /// The MSBuild ProjectUpgradeCode variable
        /// </summary>
        public string UpgradeCode { get; set; }

        /// <summary>
        /// The MSBuild ProjectUpgradeCode variable
        /// </summary>
        public Guid UpgradeCodeGuid
        {
            get
            {
                return new Guid(UpgradeCode);
            }
        }
    }
}
