using Xunit;
using WixSharp.Fluent.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WixSharp.Fluent.Extensions.Tests
{
    public class FeatureExtensionsTests
    {
        [Fact()]
        public void GetPropertyNameTest()
        {
            var feature = new Feature();
            Assert.Equal("", feature.Name);
            Assert.Equal("Feature", feature.Id);
            Assert.Equal("FEATURE_FEATURE", feature.GetPropertyName());

            feature = new Feature("Name Added.It");
            Assert.Equal("Name20Added.It", feature.Id);
            Assert.Equal("FEATURE_NAME20ADDED_IT", feature.GetPropertyName());

            feature = new Feature("Name");
            Assert.Equal("Name", feature.Id);
            Assert.Equal("FEATURE_NAME", feature.GetPropertyName());
        }

        [Fact()]
        public void SetSmartTest()
        {
            var feature = new Feature("Name Added");
            Assert.Equal("Name20Added", feature.Id);
            feature.SetSmart();
            Assert.Equal("FEATURE_NAME20ADDED = 0", feature.Condition);

            feature = new Feature("Name Added");
            feature.SetSmart();
            Assert.Equal("Name20Added.1", feature.Id);
            Assert.Equal("FEATURE_NAME20ADDED_1 = 0", feature.Condition);
        }

        [Fact()]
        public void GetConditionTest()
        {
            var feature = new Feature("Name Addedd");
            Assert.Equal("Name20Addedd", feature.Id);
            feature.SetSmart();
            Assert.Equal("(!Name20Addedd = 3 OR FEATURE_NAME20ADDEDD = 1)", feature.GetCondition());

            feature = new Feature("Name asd");
            feature.SetSmart();
            Assert.Equal("Name20asd", feature.Id);
            Assert.Equal("(!Name20asd = 3 OR FEATURE_NAME20ASD = 1)", feature.GetCondition());
        }

        [Fact()]
        public void GetConditionsTest()
        {
            Assert.Equal(" (1) ", FeatureExtensions.GetConditions());

            Assert.Equal("(!apple = 3 OR FEATURE_APPLE = 1)", 
                FeatureExtensions.GetConditions(new Feature("apple").SetSmart()));

            Assert.Equal("((!pear = 3 OR FEATURE_PEAR = 1) AND (!beer = 3 OR FEATURE_BEER = 1))", 
                FeatureExtensions.GetConditions(
                    new Feature("pear").SetSmart(),
                    new Feature("beer").SetSmart()));
        }
    }
}