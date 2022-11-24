using System;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Attribute to store the product and bundle upgrade codes
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyBundleUpgradeCodeAttribute : Attribute
    {
        /// <summary>
        /// The MSBuild BundleUpgradeCode variable
        /// </summary>
        public string UpgradeCode { get; set; }

        /// <summary>
        /// The MSBuild BundleUpgradeCode variable
        /// </summary>
        public Guid UpgradeCodeGuid { 
            get
            {
                return new Guid(UpgradeCode);
            }
        }
    }
}
