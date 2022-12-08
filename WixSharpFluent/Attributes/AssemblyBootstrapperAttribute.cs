using System;
using System.Linq;
using WixSharp.Fluent.XML;
using WixSharp;
using WixSharp.Bootstrapper;
using WixSharp.Fluent.Extensions;

namespace WixSharp.Fluent.Attributes
{
    /// <summary>
    /// Configures the installer bootstrapper
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyBootstrapperAttribute : Attribute
    {
        /// <summary>
        /// Should force hyperlink even with *.rtf license
        /// </summary>
        public string LicenseForceHyperLink { get; set; }

        /// <summary>
        /// Path to theme file ...\*.xml
        /// </summary>
        public string ThemePath { get; set; }

        /// <summary>
        /// Path to Logo file ex. ...\*.png
        /// </summary>
        public string LogoPath { get; set; }
        
        /// <summary>
        /// Path to license file ex. ...\*.rtf
        /// </summary>
        public string LicensePath { get; set; }

        /// <summary>
        /// Path to localization file ex. ...\*.wxl
        /// </summary>
        public string LocalizationPath { get; set; }

        /// <summary>
        /// Paths to additional files
        /// </summary>
        public string PayloadPaths { get; set; }

        /// <summary>
        /// Addtional payloads
        /// </summary>
        public Payload[] Payloads
        {
            get
            {
                return PayloadPaths.PathsToPathArray().Select(path => new Payload(path)).ToArray();
            }
        }

        /// <summary>
        /// The Bootstrapper generator property
        /// </summary>
        public WixStandardBootstrapperApplication Bootstrapper { 
            get
            {
                var bootstrapper = bool.Parse(LicenseForceHyperLink) ?
                  (WixStandardBootstrapperApplication)new HyperlinkLicenseBootstraperApplication() :
                  (WixStandardBootstrapperApplication)new LicenseBootstrapperApplication();

                bootstrapper.ThemeFile = ThemePath;
                bootstrapper.LogoFile = LogoPath;
                bootstrapper.LicensePath = LicensePath;
                bootstrapper.LocalizationFile = LocalizationPath;
                bootstrapper.Payloads = Payloads;

                return bootstrapper;
            }
        }
    }
}
