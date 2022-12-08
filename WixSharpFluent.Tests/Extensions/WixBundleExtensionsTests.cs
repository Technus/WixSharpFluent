using Xunit;
using System;
using System.Collections.Generic;
using WixSharp.Bootstrapper;
using Xunit.Asserts.Compare;
using System.Reflection;
using WixSharp.Fluent.Attributes;
using DLL = System.Reflection.Assembly;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class BundleTestDataGenerator
    {
        public static readonly Guid upgrade = Guid.NewGuid();

        public static IEnumerable<object[]> GetWixProjectParameters()
        {
            yield return new object[]{
                new Bundle() { Id = "Bundle", UpgradeCode = upgrade, Application = { Id = "AppBundle"} },
                new Bundle() { Id = "Bundle", UpgradeCode = upgrade, Application = { Id = "AppBundle"} },
            };
        }
    }

    public class WixBundleExtensionsTests
    {
        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetDefaultsTest(Bundle expected,Bundle provided)
        {
            expected.OutFileName = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyProductAttribute>().Product;
            expected.Name = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            expected.OutDir = "wix";
            expected.Include(WixExtension.Bal);
            expected.Include(WixExtension.Util);
            expected.Include(WixExtension.Fire);
            expected.Include(WixExtension.NetFx);
            expected.Version = new Version(DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            expected.Manufacturer = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            expected.Variables = expected.Variables.Combine(
                new Variable(
                    "InstallFolder",
                    "[ProgramFilesFolder]"+DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyProgramFilesPathAttribute>().Path)
                { 
                    Id = "Variable", 
                    Overridable = true 
                });
            provided.SetDefaults();
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetFromProjectTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetIconPathTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetIdentifiersTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetApplicationTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddNetFxWeb48Test(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddNetFxFull48Test(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddVCpp12Test(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddMsiProjectTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddInstallFolderVariableTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddFragmentTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void BuildBatchFileTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddConditionTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddVariableTest(Bundle expected, Bundle provided)
        {
            throw new NotImplementedException();
        }
    }
}