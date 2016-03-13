using Lisa.Common.WebApi;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.Generic;
using Xunit;

namespace Lisa.Common.UnitTests
{
    public class DynamicModelTest
    {
        [Fact]
        public void ItGetsAndSetsDynamicallyAssignedProperties()
        {
            dynamic model = new DynamicModel();
            model.Foo = "bar";

            Assert.Equal("bar", model.Foo);
        }

        [Fact]
        public void ItThrowsWhenGettingAPropertyThatDoesntExist()
        {
            dynamic model = new DynamicModel();
            Assert.Throws<RuntimeBinderException>(() => model.Foo);
        }

        [Fact]
        public void ItIgnoresTheCaseOfPropertyNames()
        {
            dynamic model = new DynamicModel();
            model.Foo = "bar";

            Assert.Equal("bar", model.foo);
        }

        [Fact]
        public void ItCanStoreMetadata()
        {
            dynamic model = new DynamicModel();
            object metadata = new { Foo = "Bar" };
            model.SetMetadata(metadata);

            Assert.Equal(metadata, model.GetMetadata());
        }

        [Fact]
        public void ItCanListItsMembersNames()
        {
            dynamic model = new DynamicModel();
            model.foo = 2;
            model.bar = "far";

            var memberNames = model.GetDynamicMemberNames();
            Assert.Contains("foo", memberNames);
            Assert.Contains("bar", memberNames);
        }

        [Fact]
        public void ItPreservesCaseWhenListingMemberNames()
        {
            dynamic model = new DynamicModel();
            model.FooBar = "too far";

            var memberNames = model.GetDynamicMemberNames();
            Assert.Contains("FooBar", memberNames);
        }

        [Fact]
        public void ItProvidesAccessToPropertiesThroughAnIndexer()
        {
            dynamic model = new DynamicModel();
            model.Foo = "bar";

            Assert.Equal("bar", model["Foo"]);
        }

        [Fact]
        public void ItIgnoresCaseWhenUsingTheIndexer()
        {
            dynamic model = new DynamicModel();
            model.Foo = "bar";

            Assert.Equal("bar", model["foo"]);
        }

        [Fact]
        public void ItThrowsWhenGettingAPropertyThatDoesntExistUsingTheIndexer()
        {
            dynamic model = new DynamicModel();

            Assert.Throws<KeyNotFoundException>(() => model["foo"]);
        }

        [Fact]
        public void ItCanFindOutIfAPropertyExists()
        {
            dynamic model = new DynamicModel();
            model.Foo = "bar";

            Assert.True(model.Contains("Foo"));
            Assert.False(model.Contains("bar"));
        }

        [Fact]
        public void ItIgnoresCaseWhenFindingOutIfAPropertyExists()
        {
            dynamic model = new DynamicModel();
            model.Foo = "bar";

            Assert.True(model.Contains("fOO"));
        }
    }
}