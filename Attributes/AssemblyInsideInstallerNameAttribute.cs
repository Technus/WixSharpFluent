using System;
using System.Linq;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Attribute to store the product name string
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyInsideInstallerNameAttribute : Attribute
    {
        private string productNameFull = null;

        /// <summary>
        /// The MSBuild ProductNameVersion variable
        /// </summary>
        public string ProductNameVersion { get; set; }

        /// <summary>
        /// The MSBuild ReleaseCycle variable
        /// </summary>
        public string ReleaseCycle { get; set; }

        /// <summary>
        /// The MSBuild ProductName variable
        /// </summary>
        public string ProductNameFull
        {
            get
            {
                if (productNameFull == null)
                {
                    string suffix;
                    switch (ReleaseCycle?.ToLowerInvariant())
                    {
                        default:
                            suffix = " - ALPHA"; break;
                        case "beta":
                            suffix = " - BETA"; break;
                        case "rc":
                            suffix = " - RC"; break;
                        case "rtm":
                            suffix = ""; break;
                    }

                    return $"{ProductNameVersion}{suffix}";
                }
                return productNameFull;
            }
            set
            {
                productNameFull = value;
            }
        }
    }
}
