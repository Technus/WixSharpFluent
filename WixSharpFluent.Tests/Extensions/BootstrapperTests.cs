using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixSharp.Bootstrapper;
using WixSharp.Fluent.XML;
using Xunit;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class BootstrapperTests
    {

        [Fact()]
        public void GetExtBaLocation()
        {
            string location = BootstrapperApplicationExtensions.BalExt.Assembly;
            Assert.True(System.IO.File.Exists(location));

            var bundle = new Bundle().AddExtensions(WixExtension.Bal)
                .SetApplication(new HyperlinkLicenseBootstraperApplicationExtended());
                
            bundle.Build();
        } 
    }
}
