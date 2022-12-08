using Xunit;
using WixSharp.Fluent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class GuidDictionaryTests
    {

        [Fact()]
        public void AssignGuidTest()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                ((string)null).AssignGuid(Guid.NewGuid());
            });

            Assert.Throws<ArgumentException>(() =>
            {
                " ".AssignGuid(Guid.NewGuid());
            });

            Assert.Throws<ArgumentException>(() =>
            {
                ".".AssignGuid(Guid.NewGuid());
            });

            "ID".AssignGuid(Guid.NewGuid());

            Assert.Throws<ArgumentException>(() =>
            {
                "._ID".ToId().AssignGuid(Guid.NewGuid());
            });

            "ID_ID".ToId().AssignGuid(Guid.NewGuid());
        }

        [Fact()]
        public void UnassignGuidTest()
        {
            "ID".AssignGuid(Guid.NewGuid());
            "ID".UnassignGuid();

            "ID_ID".ToId().AssignGuid(Guid.NewGuid());
            "ID_ID".ToId().UnassignGuid();
        }

        [Fact()]
        public void GenerateGuidTest()
        {
            Assert.Throws<NullReferenceException>(() =>
            {
                GuidDictionary.GenerateGuid(null);
            });

            Assert.Equal(GuidDictionary.GenerateGuid("test"), GuidDictionary.GenerateGuid("test"));
            Assert.NotEqual(GuidDictionary.GenerateGuid("test1"), GuidDictionary.GenerateGuid("test2"));

            var guid = Guid.NewGuid();

            "ID".AssignGuid(guid);
            Assert.Equal(guid, GuidDictionary.GenerateGuid("ID"));
            Assert.Equal(guid, GuidDictionary.GenerateGuid("ID"));
            "ID".UnassignGuid();
            Assert.NotEqual(guid, GuidDictionary.GenerateGuid("ID"));

            Assert.Equal(GuidDictionary.GenerateGuid("ID"), GuidDictionary.GenerateGuid("ID"));

            "ID".AssignGuid(guid);
            Assert.Equal(guid, GuidDictionary.GenerateGuid("Component.ID.EmptyDirectory"));//Some prefixes and suffixes are ignored
            Assert.Equal(guid, GuidDictionary.GenerateGuid("Component.ID.EmptyDirectory"));
            "Component.ID.EmptyDirectory".UnassignGuid();
            Assert.Equal(guid, GuidDictionary.GenerateGuid("Component.ID.EmptyDirectory"));//Some prefixes and suffixes are ignored
            Assert.Equal(guid, GuidDictionary.GenerateGuid("Component.ID.EmptyDirectory"));
            "ID".UnassignGuid();
            Assert.NotEqual(guid, GuidDictionary.GenerateGuid("Component.ID.EmptyDirectory"));

            Assert.Equal(GuidDictionary.GenerateGuid("ID"), GuidDictionary.GenerateGuid("ID"));
        }
    }
}