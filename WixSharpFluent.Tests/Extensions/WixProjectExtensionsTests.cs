using Xunit;
using WixSharp.Fluent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WixSharp.Bootstrapper;
using DLL = System.Reflection.Assembly;

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
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetMediaTemplateTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetInstallScopeTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetReinstallModeTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetIdentifiersTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetMajorUpgradeTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetIconPathTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void GetIconPathTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddEnsureTableTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddUpgradeTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetInstallExecuteSequenceTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetAdminExecuteSequenceTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void SetAdvertiseExecuteSequenceTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddFragmentTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddRemoveFolderExTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }

        [Theory()]
        [MemberData(nameof(ProjectTestDataGenerator.GetWixProjectParameters), MemberType = typeof(ProjectTestDataGenerator))]
        public void AddSmartFeaturePropertyTest(Project expected, Project provided)
        {
            throw new NotImplementedException();
        }
    }
}