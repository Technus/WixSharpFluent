using System;
using System.Linq;
using WixSharp.Fluent.XML;
using WixSharp;
using WixSharp.Bootstrapper;

namespace WixSharp.Fluent.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyBootstrapperAttribute : Attribute
    {
        public string LicenseForceHyperLink { get; set; }
        public string ThemePath { get; set; }
        public string LogoPath { get; set; }
        public string LicensePath { get; set; }
        public string LocalizationPath { get; set; }
        public string[] PayloadPaths { get; set; }
        public Payload[] Payloads
        {
            get
            {
                return PayloadPaths.Select(path => new Payload(path)).ToArray();
            }
        }

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

        public AssemblyBootstrapperAttribute(params string[] payloads)
        {
            PayloadPaths = payloads;
        }
    }
}
