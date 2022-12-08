using System.Linq;
using System.Xml.Linq;
using WixSharp.Bootstrapper;

namespace WixSharp.Fluent.XML
{
    /// <summary>
    /// Fixes the issue where License is shown in the installer instead of just a hyperlink.
    /// </summary>
    public class HyperlinkLicenseBootstraperApplication : WixStandardBootstrapperApplication
    {
        /// <inheritdoc/>
        public override XContainer[] ToXml()
        {
            XNamespace bal = Compiler.IsWix4 ?
                                "http://wixtoolset.org/schemas/v4/wxs/bal" :
                                "http://schemas.microsoft.com/wix/BalExtension";

            var root = new XElement("BootstrapperApplicationRef");

            var app = this.ToXElement(bal + "WixStandardBootstrapperApplication");

            var payloads = Payloads.ToList();

            if (LogoSideFile.IsEmpty())
                root.SetAttribute("Id", "WixStandardBootstrapperApplication.HyperlinkLicense");
            else
                root.SetAttribute("Id", "WixStandardBootstrapperApplication.HyperlinkSidebarLicense");

            if (LicensePath.IsEmpty())
                app.Add(new XAttribute("LicenseUrl", "")); //cannot use SetAttribute as we want to preserve empty attrs
            else
              if (LicensePath.StartsWith("http")) //online HTML file
                app.SetAttribute("LicenseUrl", LicensePath);
            else
            {
                app.SetAttribute("LicenseUrl", System.IO.Path.GetFileName(LicensePath));
                payloads.Add(new Payload(LicensePath));
            }

            foreach (Payload item in payloads)
            {
                var xml = item.ToXElement("Payload");
                root.AddElement(xml);
            }

            root.Add(app);

            return new[] { root };
        }
    }
}
