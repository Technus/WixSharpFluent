using Microsoft.Tools.WindowsInstallerXml.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WixSharp.Bootstrapper;
using WixSharp.Fluent.Extensions;
using DLL = System.Reflection.Assembly;

namespace WixSharp.Fluent.XML
{
    /// <summary>
    /// Uses the 'HyperlinkLicenseBootstraperApplication' and 'WixExtendedBootstrapperApplication'
    /// </summary>
    public class HyperlinkLicenseBootstraperApplicationExtended : WixStandardBootstrapperApplication
    {
        /// <inheritdoc/>
        public override XContainer[] ToXml()
        {
            XNamespace balExt = Compiler.IsWix4 ?
                                "http://wixtoolset.org/schemas/v4/wxs/bal" :
                                "http://schemas.microsoft.com/wix/BalExtension";

            var root = new XElement("BootstrapperApplicationRef");

            var app = this.ToXElement(balExt + "WixExtendedBootstrapperApplication");

            var payloads = Payloads.ToList();

            if (LogoSideFile.IsEmpty())
                root.SetAttribute("Id", "WixExtendedBootstrapperApplication.HyperlinkLicense");
            else
                root.SetAttribute("Id", "WixExtendedBootstrapperApplication.HyperlinkSidebarLicense");

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
