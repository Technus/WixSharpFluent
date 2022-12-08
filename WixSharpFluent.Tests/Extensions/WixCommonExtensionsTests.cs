using Xunit;
using System.Collections.Generic;
using Xunit.Asserts.Compare;
using WixSharp.Fluent.Attributes;
using WixSharp.Bootstrapper;
using System.Runtime.InteropServices;
using System.Reflection;
using DLL = System.Reflection.Assembly;
using System;

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
            expected.OutFileName = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            expected.Name = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            expected.OutDir = "wix";
            expected.Include(WixExtension.Bal);
            expected.Include(WixExtension.Util);
            expected.Include(WixExtension.Fire);
            expected.Include(WixExtension.NetFx);
            if (expected is Project project)
            {
                project.Version = new Version(DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
                project.ControlPanelInfo.Manufacturer = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            }
            else if (expected is Bundle bundle)
            {
                bundle.Version = new Version(DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
                bundle.Manufacturer = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            }
            provided.SetWixDefaults();
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
            provided.SetPreserveTempFiles();
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
            expected.Name = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            provided.SetName();
            DeepAssert.Equal(expected, provided);

            expected.Name = "AName";
            provided.SetName("AName");
            DeepAssert.Equal(expected, provided);

            expected.Name = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            provided.SetName(assembly: DLL.GetExecutingAssembly());
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void SetOutFileNameTest(WixProject expected, WixProject provided)
        {
            expected.OutFileName = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            provided.SetOutFileName();
            DeepAssert.Equal(expected, provided);

            expected.OutFileName = "WixSharpFluent";
            provided.SetOutFileName("WixSharpFluent");
            DeepAssert.Equal(expected, provided);

            expected.OutFileName = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            provided.SetOutFileName(assembly:DLL.GetExecutingAssembly());
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void SetVersionTest(WixProject expected, WixProject provided)
        {
            if(expected is Project project)
                project.Version = new Version(DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            else if(expected is Bundle bundle)
                bundle.Version = new Version(DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            provided.SetVersion();
            DeepAssert.Equal(expected, provided);

            if (expected is Project project2)
                project2.Version = new Version("99.0.0.0");
            else if (expected is Bundle bundle2)
                bundle2.Version = new Version("99.0.0.0");
            provided.SetVersion("99.0.0.0");
            DeepAssert.Equal(expected, provided);

            if (expected is Project project3)
                project3.Version = new Version(DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            else if (expected is Bundle bundle3)
                bundle3.Version = new Version(DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            provided.SetVersion(assembly:DLL.GetExecutingAssembly());
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(TestDataGenerator.GetWixProjectParameters), MemberType = typeof(TestDataGenerator))]
        public void SetManufacturerTest(WixProject expected, WixProject provided)
        {
            if (expected is Project project)
                project.ControlPanelInfo.Manufacturer = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            else if (expected is Bundle bundle)
                bundle.Manufacturer = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            provided.SetManufacturer();
            DeepAssert.Equal(expected, provided);

            if (expected is Project project2)
                project2.ControlPanelInfo.Manufacturer = "99.0.0.0";
            else if (expected is Bundle bundle2)
                bundle2.Manufacturer ="99.0.0.0";
            provided.SetManufacturer("99.0.0.0");
            DeepAssert.Equal(expected, provided);

            if (expected is Project project3)
                project3.ControlPanelInfo.Manufacturer = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            else if (expected is Bundle bundle3)
                bundle3.Manufacturer = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            provided.SetManufacturer(assembly: DLL.GetExecutingAssembly());
            DeepAssert.Equal(expected, provided);
        }
    }
}