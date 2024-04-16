using Xunit;
using WixSharp.Fluent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixSharp.Bootstrapper;
using DLL = System.Reflection.Assembly;
using System.Reflection;
using WixSharp.Fluent.Attributes;
using Xunit.Asserts.Compare;
using Xunit.Sdk;
using static WixSharp.Fluent.Extensions.Tests.AssemblyAttributeExtensionsTests;
using WixSharp.CommonTasks;
using WixSharp.Fluent.XML;
using Microsoft.Deployment.WindowsInstaller;
using System.Xml.Linq;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class ProjectTestDataGenerator
    {
        public static readonly Feature defaultFeature = new Feature("Complete");
        public static readonly Guid upgrade = Guid.NewGuid();

        public static IEnumerable<object[]> GetWixProjectParameters()
        {
            yield return new object[]{
                new Project() { DefaultFeature = defaultFeature , Id = "Project", UpgradeCode = upgrade, ControlPanelInfo = { Id = "CpProject" }, Package = { Id = "PackageId" } },
                new Project() { DefaultFeature = defaultFeature , Id = "Project", UpgradeCode = upgrade, ControlPanelInfo = { Id = "CpProject" }, Package = { Id = "PackageId" } },
            };
        }
    }

    public class WixProjectExtensionsTests
    {
        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetDefaultsTest(Project expected, Project provided)
        {
            expected.OutFileName = testAssembly.GetCustomAttribute<AssemblyProductAttribute>().Product;
            expected.Name = testAssembly.GetCustomAttribute<AssemblyInsideInstallerNameAttribute>().ProductNameFull;
            expected.OutDir = "bin";
            expected.Include(WixExtension.Bal);
            expected.Include(WixExtension.Util);
            expected.Include(WixExtension.Fire);
            expected.Include(WixExtension.NetFx);
            expected.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            expected.ControlPanelInfo.Manufacturer = testAssembly.GetCustomAttribute<AssemblyCompanyAttribute>().Company;
            expected.GUID = testAssembly.GetCustomAttribute<AssemblyProjectUpgradeCodeAttribute>().UpgradeCodeGuid;
            expected.UpgradeCode = expected.GUID;
            expected.ProductId = Project.CalculateProductId((Guid)expected.GUID, expected.Version);
            expected.Id = ((Guid)expected.ProductId).ToValidWixId();
            expected.MajorUpgrade = new MajorUpgrade
            {
                DowngradeErrorMessage = "A newer version is already installed.",
                Schedule = UpgradeSchedule.afterInstallValidate,
                AllowSameVersionUpgrades = false,
            };
            expected.Add(new MediaTemplate()
            {
                EmbedCab = true,
            });
            expected.Media.Clear();
            expected.InstallScope = InstallScope.perMachine;
            expected.ControlPanelInfo.ProductIcon = testAssembly.GetCustomAttribute<AssemblyIconPathAttribute>().Path;
            provided.SetDefaults(noThrow: false, assembly: testAssembly);
#if DEBUG
            expected.PreserveTempFiles = true;
#else
            expected.PreserveTempFiles = false;
#endif
            DeepAssert.Equal(expected, provided, "GenericItems","Properties", "ControlPanelInfo");
            DeepAssert.Equal(expected.ControlPanelInfo, provided.ControlPanelInfo, "ProductIcon");
            DeepAssert.Equal(expected.GenericItems, provided.GenericItems, "Id");
            DeepAssert.Equal(expected.Properties, provided.Properties, "Id");
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetMediaTemplateTest(Project expected, Project provided)
        {
            expected.Add(new MediaTemplate()
            {
                EmbedCab = true,
            });
            expected.Media.Clear();
            provided.SetMediaTemplate();
            DeepAssert.Equal(expected, provided, "GenericItems");
            DeepAssert.Equal(expected.GenericItems, provided.GenericItems, "Id");

            expected.Add(new MediaTemplate()
            {
                EmbedCab = false,
            });
            expected.Media.Clear();
            provided.SetMediaTemplate(embedCab:false);
            DeepAssert.Equal(expected, provided, "GenericItems");
            DeepAssert.Equal(expected.GenericItems, provided.GenericItems, "Id");
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetInstallScopeTest(Project expected, Project provided)
        {
            expected.InstallScope = InstallScope.perMachine;
            provided.SetInstallScope();
            DeepAssert.Equal(expected, provided);

            expected.InstallScope = InstallScope.perUser;
            provided.SetInstallScope(InstallScope.perUser);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetReinstallModeTest(Project expected, Project provided)
        {
            Assert.Equal("omus", expected.ReinstallMode);//sanity check
            provided.SetReinstallMode();//set to default
            DeepAssert.Equal(expected, provided);

            expected.ReinstallMode = "amus";
            provided.SetReinstallMode(ReinstallMode.EveryFile + ReinstallMode.RewriteMachineRegistry + ReinstallMode.RewriteUserRegistry + ReinstallMode.RewriteShortcuts);
            DeepAssert.Equal(expected, provided);

            expected.ReinstallMode = "omus";
            provided.SetReinstallMode();//set to default
            DeepAssert.Equal(expected, provided);

            expected.ReinstallMode = "amus";
            provided.SetReinstallMode(ReinstallMode.EveryFile,ReinstallMode.RewriteMachineRegistry,ReinstallMode.RewriteUserRegistry,ReinstallMode.RewriteShortcuts);
            DeepAssert.Equal(expected, provided);

            Assert.Throws<ArgumentException>(() =>
            {
                provided.SetReinstallMode(ReinstallMode.EveryFile, ReinstallMode.EveryFile);
            });
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetIdentifiersTest(Project expected, Project provided)
        {
            expected.Version = new Version(testAssembly.GetCustomAttribute<AssemblyFileVersionAttribute>().Version);
            expected.GUID = testAssembly.GetCustomAttribute<AssemblyProjectUpgradeCodeAttribute>().UpgradeCodeGuid;
            expected.UpgradeCode = expected.GUID;
            expected.ProductId = Project.CalculateProductId((Guid)expected.GUID, expected.Version);
            expected.Id = ((Guid)expected.ProductId).ToValidWixId();
            provided.SetIdentifiers(assembly:testAssembly);//set to default
            DeepAssert.Equal(expected, provided);

            var guid1 = Guid.NewGuid();
            var guid2 = Guid.NewGuid();
            var ver = "1.0.0.98";

            expected.Version = new Version(ver);
            expected.GUID = guid2;
            expected.UpgradeCode = guid1;
            expected.ProductId = Project.CalculateProductId((Guid)expected.GUID, expected.Version);
            expected.Id = ((Guid)expected.ProductId).ToValidWixId();
            provided.SetIdentifiers(guid1,guid2,ver);//set to default
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetMajorUpgradeTest(Project expected, Project provided)
        {
            expected.MajorUpgrade = new MajorUpgrade
            {
                DowngradeErrorMessage = "A newer version is already installed.",
                Schedule = UpgradeSchedule.afterInstallValidate,
                AllowSameVersionUpgrades = false,
            };
            provided.SetMajorUpgrade();
            DeepAssert.Equal(expected, provided);

            expected.MajorUpgrade = new MajorUpgrade
            {
                DowngradeErrorMessage = "Masło",
                Schedule = UpgradeSchedule.afterInstallValidate,
                AllowSameVersionUpgrades = false,
            };
            provided.SetMajorUpgrade("Masło");
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetIconPathTest(Project expected, Project provided)
        {
            var props = expected.Properties;
            expected.ControlPanelInfo.ProductIcon = testAssembly.GetCustomAttribute<AssemblyIconPathAttribute>().Path;
            provided.SetIconPath(assembly:testAssembly);
            DeepAssert.Equal(expected, provided, "Properties", "ControlPanelInfo");
            DeepAssert.Equal(expected.ControlPanelInfo, provided.ControlPanelInfo, "ProductIcon");
            DeepAssert.Equal(expected.Properties, provided.Properties, "Id");

            expected.ControlPanelInfo.ProductIcon = "BUTTER!!!";
            provided.SetIconPath("BUTTER!!!");
            DeepAssert.Equal(expected, provided, "Properties", "ControlPanelInfo");
            DeepAssert.Equal(expected.ControlPanelInfo, provided.ControlPanelInfo, "ProductIcon");
            DeepAssert.Equal(expected.Properties, provided.Properties, "Id");
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void GetIconPathTest(Project expected, Project provided)
        {
            expected.SetIconPath("A");
            provided.SetIconPath("A");
            DeepAssert.Equal(expected, provided, "Properties");
            DeepAssert.Equal(expected.Properties, provided.Properties, "Id");//sanity
            Assert.Equal(expected.GetIconPath(), provided.GetIconPath());
            Assert.Equal("A", expected.GetIconPath());
            Assert.Equal("A", expected.GetIconPath());
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddEnsureTableTest(Project expected, Project provided)
        {
            expected.AddXmlElement("Wix/Product", "EnsureTable", $"Id=ENSURE_THIS!");
            provided.AddEnsureTable("ENSURE_THIS!");
            DeepAssert.Equal(expected, provided);

            expected.AddXmlElement("Wix/Product", "EnsureTable", $"Id=ENSURE_THAT!");
            provided.AddEnsureTable("ENSURE_THAT!");
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddUpgradeTest(Project expected, Project provided)
        {
            //TODO Uncheckable action... some test after generation of XML is need...
            provided.AddUpgrade();
            DeepAssert.Equal(expected, provided);

            provided.UpgradeCode = null;
            Assert.Throws<ArgumentException>(() =>
            {
                provided.AddUpgrade();
            });
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetInstallExecuteSequenceTest(Project expected, Project provided)
        {
            //TODO Uncheckable action... some test after generation of XML is need...
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetAdminExecuteSequenceTest(Project expected, Project provided)
        {
            //TODO Uncheckable action... some test after generation of XML is need...
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetAdvertiseExecuteSequenceTest(Project expected, Project provided)
        {
            //TODO Uncheckable action... some test after generation of XML is need...
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddFragmentTest(Project expected, Project provided)
        {
            var xml = new UtilRegistrySearch();
            xml.Key = "Cucumber";
            expected.AddWixFragment("Wix/Product", xml);
            provided.AddFragment(xml);
            DeepAssert.Equal(expected, provided);

            xml = new UtilRegistrySearch();
            xml.Variable = "Coolaid";
            expected.AddWixFragment("Wix/Product", xml);
            provided.AddFragment(xml);
            DeepAssert.Equal(expected, provided);
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddRemoveFolderExTest(Project expected, Project provided)
        {
            //TODO
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddSmartFeaturePropertyTest(Project expected, Project provided)
        {
            var feature = new Feature("feat").SetSmart();
            expected.AddProperty(new Property(feature.GetPropertyName(), "1"));
            provided.AddSmartFeatureProperty(feature);
            DeepAssert.Equal(expected, provided, "Properties");
            DeepAssert.Equal(expected.Properties, provided.Properties, "Id");

            feature = new Feature("featurezz").SetSmart();
            expected.AddProperty(new Property(feature.GetPropertyName(), "LEET"));
            provided.AddSmartFeatureProperty(feature, "LEET");
            DeepAssert.Equal(expected, provided, "Properties");
            DeepAssert.Equal(expected.Properties, provided.Properties, "Id");

            feature = new Feature("featuress").SetSmart("KEK");
            expected.AddProperty(new Property("KEK", "LEET"));
            provided.AddSmartFeatureProperty(feature, "LEET");
            DeepAssert.Equal(expected, provided, "Properties");
            DeepAssert.Equal(expected.Properties, provided.Properties, "Id");
        }
    }
}