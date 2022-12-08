using Xunit;
using WixSharp.Fluent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class IdExtensionsTests
    {
        [Fact()]
        public void ToValidWixIdTest()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                IdExtensions.ToValidWixId(null);
            });

            var guid = new Guid("714A7EF7-05F6-40BC-9EF2-8A14E37A2717");

            Assert.Equal("_714a7ef705f640bc9ef28a14e37a2717", IdExtensions.ToValidWixId(guid));
        }
    }
}