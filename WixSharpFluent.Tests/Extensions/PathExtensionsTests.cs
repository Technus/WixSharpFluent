using Xunit;
using WixSharp.Fluent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class PathExtensionsTests
    {
        [Theory()]
        [InlineData(@"asd\asd", @"asd\asd", @"asd\asd"                     , ';',1)]
        [InlineData(@"asd\asd", @"asd\asd", @";asd/asd"                    , ';',1)]
        [InlineData(@"asd\asd", @"asd\asd", @";;;;;asd/asd"                , ';',1)]
        [InlineData(@"asda"   , @"asd\asd", @";;asda;;;asd\asd"            , ';',2)]
        [InlineData(@"asdff"  , @"asd\asd", @";;asdff;;;asd/asd"           , ';',2)]
        [InlineData(@"asdff"  , @"asd\asd", @";;asdff;;;asd\asd;;;;;;"     , ';',2)]
        [InlineData(@"asdff"  , @"dd"     , @";;asdff;;;asd\asd;;;asd;dd;;", ';',4)]
        public void PathSplitCleanupTest(string first,string last,string value,char separator,int count)
        {
            var split = value.PathSplitCleanup(separator);
            Assert.Equal(first, split.First());
            Assert.Equal(last, split.Last());
            Assert.Equal(count, split.Count());
        }
    }
}