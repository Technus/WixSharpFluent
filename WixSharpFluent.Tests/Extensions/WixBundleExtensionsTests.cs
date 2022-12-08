using Xunit;
using System;
using System.Collections.Generic;
using WixSharp.Bootstrapper;
using Xunit.Asserts.Compare;
using System.Reflection;
using WixSharp.Fluent.Attributes;
using DLL = System.Reflection.Assembly;
using WixSharp.Fluent.XML;

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
            expected.UpgradeCode = new Guid("49D61823-8433-4CD4-A9BD-669F695587D0");

            WixStandardBootstrapperApplication app;
            if (bool.Parse(DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyBootstrapperAttribute>().LicenseForceHyperLink))
            {
                app = new HyperlinkLicenseBootstraperApplication();
            }
            else
            {
                app = new HyperlinkLicenseBootstraperApplication();
            }
            var bootstrapperAttribute = DLL.GetExecutingAssembly().GetCustomAttribute<AssemblyBootstrapperAttribute>();

            app.LicensePath = bootstrapperAttribute.LicensePath;
            app.LogoFile = bootstrapperAttribute.LogoPath;
            app.ThemeFile = bootstrapperAttribute.ThemePath;
            app.LocalizationFile = bootstrapperAttribute.LocalizationPath;
            app.Payloads = app.Payloads.Combine(bootstrapperAttribute.Payloads);

            expected.Application = app;
            
            provided.SetDefaults(noThrow:false);
            DeepAssert.Equal(expected, provided,"Application");
            DeepAssert.Equal(expected.Application, provided.Application,"Id");
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetFromProjectTest(Bundle expected, Bundle provided)
        {
            var project = new Project()
            {
                
            }.SetDefaults(noThrow: false);

            expected.OutDir = project.OutDir;
            expected.OutFileName = project.OutFileName+" Setup";
            expected.WixExtensions.AddRange(project.WixExtensions);
            expected.IconFile = project.GetIconPath();
            expected.Manufacturer = project.ControlPanelInfo.Manufacturer;
            expected.Version = project.Version;
            expected.CustomIdAlgorithm = project.CustomIdAlgorithm;
            expected.Name = project.Name;
            expected.PreserveTempFiles = project.PreserveTempFiles;
            expected.SourceBaseDir = project.SourceBaseDir;

            provided.SetFromProject(project);
            DeepAssert.Equal(expected, provided, "Variables");
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