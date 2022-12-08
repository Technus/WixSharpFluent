using Xunit;
using System;
using static WixSharp.Fluent.Extensions.AssemblyAttributeExtensions;
using DLL = System.Reflection.Assembly;
using WixSharp.Fluent.Attributes;

[assembly: AssemblyExecutableName(ExecutableName = "Test")]

#if DEBUG
[assembly: AssemblyDefines(DefineConstants = "DEBUG")]
#else
[assembly: AssemblyDefines(DefineConstants = "")]
#endif

[assembly: AssemblyInsideInstallerName(ProductNameFull = "FullName")]
[assembly: AssemblyProgramFilesPath(Path="testing")]
[assembly: AssemblyStartMenuPath(Path="testing")]

namespace WixSharp.Fluent.Extensions.Tests
{
    public class AssemblyAttributeExtensionsTests
    {
        [Fact()]
        public void GetOtherCallingAssemblyTest()
        {
            var thisDLL = DLL.GetExecutingAssembly();

            AssembliesToIgnoreWhileLookingUp.Remove(thisDLL);

            var dll = GetOtherCallingAssembly();
            Assert.True(dll is DLL,"Must Find some Assembly");
            Assert.True(dll.Equals(thisDLL),"Pinpoint this Assembly");

            AssembliesToIgnoreWhileLookingUp.Add(thisDLL);

            dll = GetOtherCallingAssembly();
            Assert.True(dll is DLL, "Must Find some Assembly");
            Assert.False(dll.Equals(thisDLL), "Pinpoint other Assembly");
        }

        [Fact()]
        public void GetAssemblyAttributeTest()
        {
            var thisDLL = DLL.GetExecutingAssembly();

            AssembliesToIgnoreWhileLookingUp.Remove(thisDLL);

            var attr = GetAssemblyAttribute<AssemblyExecutableNameAttribute>();
            Assert.True(attr is AssemblyExecutableNameAttribute, "Check returned type");
            Assert.True("Test".Equals(attr.ExecutableName), "Check content");

            AssembliesToIgnoreWhileLookingUp.Add(thisDLL);

            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                  attr = GetAssemblyAttribute<AssemblyExecutableNameAttribute>();
                }
                catch (ArgumentException ex)
                {
                    attr = null;
                    throw;
                }
            });

            Assert.False(attr is AssemblyExecutableNameAttribute, "Check returned type");

            attr = GetAssemblyAttribute<AssemblyExecutableNameAttribute>(noThrow:true);
            Assert.False(attr is AssemblyExecutableNameAttribute, "Check returned type");

            attr = GetAssemblyAttribute<AssemblyExecutableNameAttribute>(assembly:thisDLL);//Direct dll ref
            Assert.True(attr is AssemblyExecutableNameAttribute, "Check returned type");
            Assert.True("Test".Equals(attr.ExecutableName), "Check content");
        }
    }
}