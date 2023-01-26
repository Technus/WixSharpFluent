using Xunit;
using System;
using System.Collections.Generic;
using WixSharp.Bootstrapper;
using Xunit.Asserts.Compare;
using System.Reflection;
using WixSharp.Fluent.Attributes;
using DLL = System.Reflection.Assembly;
using WixSharp.Fluent.XML;
using Xunit.Sdk;
using static WixSharp.Fluent.Extensions.Tests.AssemblyAttributeExtensionsTests;
using System.IO;
using System.Linq;
using WixSharp.CommonTasks;

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
            expected.OutFileName = testAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            expected.Name = testAssembly.GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            expected.OutDir = "bin";
            expected.Include(WixExtension.Bal);
            expected.Include(WixExtension.Util);
            expected.Include(WixExtension.Fire);
            expected.Include(WixExtension.NetFx);
            expected.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            expected.Manufacturer = testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            expected.Variables = expected.Variables.Combine(
                new Variable(
                    "InstallFolder",
                    "[ProgramFilesFolder]"+testAssembly.GetCustomAttribute<AssemblyProgramFilesPathAttribute>().Path)
                { 
                    Id = "Variable", 
                    Overridable = true 
                });
            expected.UpgradeCode = new Guid("49D61823-8433-4CD4-A9BD-669F695587D0");
#if DEBUG
            expected.PreserveTempFiles = true;
#else
            expected.PreserveTempFiles = false;
#endif

            WixStandardBootstrapperApplication app;
            if (bool.Parse(testAssembly.GetCustomAttribute<AssemblyBootstrapperAttribute>().LicenseForceHyperLink))
            {
                app = new HyperlinkLicenseBootstraperApplication();
            }
            else
            {
                app = new HyperlinkLicenseBootstraperApplication();
            }
            var bootstrapperAttribute = testAssembly.GetCustomAttribute<AssemblyBootstrapperAttribute>();

            app.LicensePath = bootstrapperAttribute.LicensePath;
            app.LogoFile = bootstrapperAttribute.LogoPath;
            app.ThemeFile = bootstrapperAttribute.ThemePath;
            app.LocalizationFile = bootstrapperAttribute.LocalizationPath;
            app.Payloads = app.Payloads.Combine(bootstrapperAttribute.Payloads);

            expected.Application = app;
            
            provided.SetDefaults(noThrow: false, assembly: testAssembly);
            DeepAssert.Equal(expected, provided,"Application", "IconFile");
            DeepAssert.Equal(expected.Application, provided.Application,"Id");
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetFromProjectTest(Bundle expected, Bundle provided)
        {
            var project = new Project().SetDefaults(noThrow: false, assembly: testAssembly);

            expected.OutDir = project.OutDir;
            expected.OutFileName = project.OutFileName;
            expected.WixExtensions.AddRange(project.WixExtensions);
            expected.IconFile = project.GetIconPath();
            expected.Manufacturer = project.ControlPanelInfo.Manufacturer;
            expected.Version = project.Version;
            expected.CustomIdAlgorithm = project.CustomIdAlgorithm;
            expected.Name = project.Name;
            expected.PreserveTempFiles = project.PreserveTempFiles;
            expected.SourceBaseDir = project.SourceBaseDir;

            provided.SetFromProject(project, assembly: testAssembly);
            DeepAssert.Equal(expected, provided, "Variables");
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetIconPathTest(Bundle expected, Bundle provided)
        {
            expected.IconFile = testAssembly.GetCustomAttribute<AssemblyIconPathAttribute>().Path;
            provided.SetIconPath(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);

            expected.IconFile = "BOOP.ico";
            provided.SetIconPath("BOOP.ico");
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetIdentifiersTest(Bundle expected, Bundle provided)
        {
            expected.UpgradeCode = testAssembly.GetCustomAttribute<AssemblyBundleUpgradeCodeAttribute>().UpgradeCodeGuid;
            provided.SetIdentifiers(assembly: testAssembly);
            DeepAssert.Equal(expected, provided);

            var guid = Guid.NewGuid();
            expected.UpgradeCode = guid;
            provided.SetIdentifiers(guid);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void SetApplicationTest(Bundle expected, Bundle provided)
        {
            expected.Application = testAssembly.GetCustomAttribute<AssemblyBootstrapperAttribute>().Bootstrapper;
            provided.SetApplication(assembly: testAssembly);
            DeepAssert.Equal(expected, provided, "Application");
            DeepAssert.Equal(expected.Application, provided.Application, "Id");

            var app = new LicenseBootstrapperApplication();

            expected.Application = app;
            provided.SetApplication(app);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddInstallFolderVariableTest(Bundle expected, Bundle provided)
        {
            var variables = expected.Variables;

            var installFolder = new Variable("InstallFolder", "[ProgramFilesFolder]"+ testAssembly.GetCustomAttribute<AssemblyProgramFilesPathAttribute>().Path)
            {
                Overridable = true
            };
            expected.Variables = variables.Combine(installFolder).ToArray();

            provided.AddInstallFolderVariable(assembly: testAssembly);
            DeepAssert.Equal(expected, provided,"Variables");
            DeepAssert.Equal(expected.Variables, provided.Variables,"Id");

            provided.Variables = new Variable[0];

            installFolder.Value = "installPath";

            provided.AddInstallFolderVariable("installPath","");
            DeepAssert.Equal(expected, provided, "Variables");
            DeepAssert.Equal(expected.Variables, provided.Variables, "Id");
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddFragmentTest(Bundle expected, Bundle provided)
        {
            var xml = new UtilRegistrySearch();
            xml.Key = "Cucumber";
            expected.AddWixFragment("Wix/Bundle", xml);
            provided.AddFragment(xml);
            DeepAssert.Equal(expected, provided);

            xml = new UtilRegistrySearch();
            xml.Variable = "Coolaid";
            expected.AddWixFragment("Wix/Bundle", xml);
            provided.AddFragment(xml);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddConditionTest(Bundle expected, Bundle provided)
        {
            var xmlNamespace = WixExtension.Bal.XmlNamespace;
            expected.AddXml("Wix/Bundle", $@"<Condition xmlns=""{xmlNamespace}"" Message=""nope""> (1) </Condition>");
            provided.AddCondition("nope",Condition.Always);
            DeepAssert.Equal(expected, provided);

            expected.AddXml("Wix/Bundle", $@"<Condition xmlns=""{xmlNamespace}"" Message=""hide""> (UILevel < 4) </Condition>");
            provided.AddCondition("hide", Condition.Silent);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(BundleTestDataGenerator.GetWixProjectParameters), MemberType = typeof(BundleTestDataGenerator))]
        public void AddVariableTest(Bundle expected, Bundle provided)
        {
            var variable = new Variable("keep");
            expected.Variables = expected.Variables.Combine(variable);
            provided.AddVariable(variable);
            DeepAssert.Equal(expected, provided);

            variable = new Variable("keepnt");
            variable.Value = "butter";
            expected.Variables = expected.Variables.Combine(variable);
            provided.AddVariable(variable);
            DeepAssert.Equal(expected, provided);
        }
    }
}