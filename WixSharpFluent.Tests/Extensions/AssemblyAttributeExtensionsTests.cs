using Xunit;
using System;
using static WixSharp.Fluent.Extensions.AssemblyAttributeExtensions;
using DLL = System.Reflection.Assembly;
using WixSharp.Fluent.Attributes;
using AssemblyAttributesTestHolder;

namespace WixSharp.Fluent.Extensions.Tests
{

    public class AssemblyAttributeExtensionsTests
    {
        public static readonly DLL testAssembly = typeof(CallMe).Assembly;

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

            AssembliesToIgnoreWhileLookingUp.Remove(thisDLL);
        }

        [Fact()]
        public void GetAssemblyAttributeTest()
        {
            var thisDLL = DLL.GetExecutingAssembly();

            AssembliesToIgnoreWhileLookingUp.Remove(thisDLL);

            var attr = GetAssemblyAttribute<AssemblyExecutableNameAttribute>(assembly:testAssembly);
            Assert.True(attr is AssemblyExecutableNameAttribute, "Check returned type");
            Assert.True("Test".Equals(attr.ExecutableName), "Check content");

            AssembliesToIgnoreWhileLookingUp.Add(thisDLL);

            Assert.Throws<ArgumentException>(() =>
            {
                try
                {
                  attr = GetAssemblyAttribute<AssemblyExecutableNameAttribute>();
                }
                catch (ArgumentException)
                {
                    attr = null;
                    throw;
                }
            });

            Assert.False(attr is AssemblyExecutableNameAttribute, "Check returned type");

            attr = GetAssemblyAttribute<AssemblyExecutableNameAttribute>(noThrow:true);
            Assert.False(attr is AssemblyExecutableNameAttribute, "Check returned type");

            attr = GetAssemblyAttribute<AssemblyExecutableNameAttribute>(assembly:testAssembly);
            Assert.True(attr is AssemblyExecutableNameAttribute, "Check returned type");
            Assert.True("Test".Equals(attr.ExecutableName), "Check content");

            AssembliesToIgnoreWhileLookingUp.Remove(thisDLL);
        }
    }
}