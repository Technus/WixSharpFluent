using Xunit;
using System.Collections.Generic;
using Xunit.Asserts.Compare;
using WixSharp.Fluent.Attributes;
using WixSharp.Bootstrapper;
using System.Runtime.InteropServices;
using System.Reflection;
using DLL = System.Reflection.Assembly;
using System;
using static WixSharp.Fluent.Extensions.Tests.AssemblyAttributeExtensionsTests;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class TestDataGenerator
    {
        public static IEnumerable<object[]> GetWixProjectParameters()
        {
            foreach (var item in BundleTestDataGenerator.GetWixProjectParameters())
                yield return item;
            foreach (var item in ProjectTestDataGenerator.GetWixProjectParameters())
                yield return item;
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void GeneratorsTest(WixProject expected, WixProject provided)
        {
            DeepAssert.Equal(expected, provided);
        }
    }

    public class WixCommonExtensionsTests
    {
        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters),MemberType = typeof(TestDataGenerator))]
        public void SetWixDefaultsTest(WixProject expected,WixProject provided)
        {
            expected.OutFileName = testAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            expected.Name = testAssembly.GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            expected.OutDir = "bin";
            expected.Include(WixExtension.Bal);
            expected.Include(WixExtension.Util);
            expected.Include(WixExtension.Fire);
            expected.Include(WixExtension.NetFx);
            if (expected is Project project)
            {
                project.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
                project.ControlPanelInfo.Manufacturer = testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            }
            else if (expected is Bundle bundle)
            {
                bundle.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
                bundle.Manufacturer = testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            }
#if DEBUG
            expected.PreserveTempFiles = true;
#else
            expected.PreserveTempFiles = false;
#endif
            provided.SetWixDefaults(noThrow: false, assembly: testAssembly);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void AddExtensionsTest(WixProject expected, WixProject provided)
        {
            provided.AddExtensions();
            DeepAssert.Equal(expected, provided);

            expected.Include(WixExtension.Fire);
            provided.AddExtensions(WixExtension.Fire);
            DeepAssert.Equal(expected, provided);

            expected.Include(WixExtension.Bal);
            provided.AddExtensions(WixExtension.Bal);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void SetPreserveTempFilesTest(WixProject expected, WixProject provided)
        {
#if DEBUG
            expected.PreserveTempFiles = true;
#else
            expected.PreserveTempFiles = false;
#endif
            provided.SetPreserveTempFiles(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);

            expected.PreserveTempFiles = true;
            provided.SetPreserveTempFiles(true);
            DeepAssert.Equal(expected, provided);

            expected.PreserveTempFiles = false;
            provided.SetPreserveTempFiles(false);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void SetNameTest(WixProject expected, WixProject provided)
        {
            expected.Name = testAssembly.GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            provided.SetName(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);

            expected.Name = "AName";
            provided.SetName("AName");
            DeepAssert.Equal(expected, provided);

            expected.Name = testAssembly.GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            provided.SetName(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void SetOutFileNameTest(WixProject expected, WixProject provided)
        {
            expected.OutFileName = testAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            provided.SetOutFileName(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);

            expected.OutFileName = "WixSharpFluent";
            provided.SetOutFileName("WixSharpFluent");
            DeepAssert.Equal(expected, provided);

            expected.OutFileName = testAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            provided.SetOutFileName(assembly:testAssembly);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void SetVersionTest(WixProject expected, WixProject provided)
        {
            if(expected is Project project)
                project.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            else if(expected is Bundle bundle)
                bundle.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            provided.SetVersion(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);

            if (expected is Project project2)
                project2.Version = new Version("99.0.0.0");
            else if (expected is Bundle bundle2)
                bundle2.Version = new Version("99.0.0.0");
            provided.SetVersion("99.0.0.0");
            DeepAssert.Equal(expected, provided);

            if (expected is Project project3)
                project3.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            else if (expected is Bundle bundle3)
                bundle3.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            provided.SetVersion(assembly:testAssembly);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void SetManufacturerTest(WixProject expected, WixProject provided)
        {
            if (expected is Project project)
                project.ControlPanelInfo.Manufacturer = testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            else if (expected is Bundle bundle)
                bundle.Manufacturer = testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            provided.SetManufacturer(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);

            if (expected is Project project2)
                project2.ControlPanelInfo.Manufacturer = "99.0.0.0";
            else if (expected is Bundle bundle2)
                bundle2.Manufacturer ="99.0.0.0";
            provided.SetManufacturer("99.0.0.0");
            DeepAssert.Equal(expected, provided);

            if (expected is Project project3)
                project3.ControlPanelInfo.Manufacturer = testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            else if (expected is Bundle bundle3)
                bundle3.Manufacturer = testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            provided.SetManufacturer(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);
        }
    }
}