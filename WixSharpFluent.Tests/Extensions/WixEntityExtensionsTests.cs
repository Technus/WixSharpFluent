using Xunit;
using WixSharp.Fluent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Asserts.Compare;
using static WixSharp.Fluent.Extensions.WixEntityExtensions;
using WixSharp.Fluent.XML;
using WixSharp.CommonTasks;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class WixEntityExtensionsTests
    {
        [Fact()]
        public void GetLastDirTest()
        {
            var d = new Dir(@"eat\the\path");
            Assert.Equal("eat", d.Name);
            Assert.Equal("path", d.GetLastDir().Name);

            d = new Dir(@"boon");
            Assert.Equal("boon", d.Name);
            Assert.Equal("boon", d.GetLastDir().Name);

            DeepAssert.Equal(d, d.GetLastDir());
        }

        [Fact()]
        public void GetPermissionForEveryoneTest()
        {
            var perms = new DirPermission()
            {
                User = "Everyone",
                GenericAll = true,
            };

            DeepAssert.Equal(perms, GetPermissionForEveryone(),"Id");
        }

        [Fact()]
        public void AddPermisssionEveryoneTest()
        {
            var perms = new DirPermission()
            {
                User = "Everyone",
                GenericAll = true,
            };

            var expected = new Dir("one");
            expected.Permissions = expected.Permissions.Combine(perms);

            var provided = new Dir("one");
            provided.AddPermisssionEveryone();

            DeepAssert.Equal(expected, provided, "Id");
        }

        [Fact()]
        public void GetCreateRemoveFolderTest()
        {
            DeepAssert.Equal(new CreateFolder(),GetCreateRemoveFolder(), "Id");
        }

        [Fact()]
        public void AddCreateRemoveTest()
        {
            var expected = new Dir("one");
            expected.Add(GetCreateRemoveFolder());

            var provided = new Dir("one");
            provided.AddCreateRemove();

            DeepAssert.Equal(expected, provided, "Id");
        }

        [Fact()]
        public void WithLastDirTest()
        {
            var d = new Dir(@"eat\the\path");
            d.WithLastDir(dir =>
            {
                Assert.Equal("path", dir.Name);
                DeepAssert.Equal(dir,d.GetLastDir());
            });
        }

        [Fact()]
        public void SetFireWallExceptionTest()
        {
            var expected = new File("filee.exx");
            var name = Path.GetFileNameWithoutExtension("filee.exx");
            expected.FirewallExceptions = expected.FirewallExceptions.Combine(new FirewallException()
            {
                Scope = FirewallExceptionScope.any,
                IgnoreFailure = true,
                Profile = FirewallExceptionProfile.all,
                Name = name,
            });
            var provided = new File("filee.exx");
            provided.SetFireWallException();
            DeepAssert.Equal(expected,provided, "Id");
        }

        [Fact()]
        public void SetKeyPathTest()
        {
            var expected = new File("filee.exx");
            expected.Attributes.Add("KeyPath", "yes");

            var provided = new File("filee.exx");
            provided.SetKeyPath();

            DeepAssert.Equal(expected, provided, "Id");

            expected = new File("fileezz.exx");
            expected.Attributes.Add("KeyPath", "no");

            provided = new File("fileezz.exx");
            provided.SetKeyPath(false);

            DeepAssert.Equal(expected, provided, "Id");

            expected = new File("fileezz.exx");
            expected.Attributes.Add("KeyPath", "yes");

            provided = new File("fileezz.exx");
            provided.SetKeyPath(true);

            DeepAssert.Equal(expected, provided, "Id");
        }

        [Fact()]
        public void SetFeatureAndConditionTest()
        {
            var feature = new Feature("fact");

            //The '!' operator reads the feature state
            var condition = feature.GetCondition();

            var expected = new File("fil.fil");

            expected.Feature = feature;
            expected.ComponentCondition = condition;

            var provided = new File("fil.fil")
                .SetFeatureAndCondition(feature);

            DeepAssert.Equal(expected,provided, "Id");

            var feature2 = new Feature("fact2");

            //The '!' operator reads the feature state
            condition = FeatureExtensions.GetConditions(feature,feature2);

            expected = new File("fil.fil");

            expected.Feature = feature;
            expected.ComponentCondition = condition;

            provided = new File("fil.fil")
                .SetFeatureAndCondition(feature,condition);

            DeepAssert.Equal(expected, provided, "Id");
        }
    }
}