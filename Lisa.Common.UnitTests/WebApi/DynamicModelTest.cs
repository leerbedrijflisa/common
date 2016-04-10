using Lisa.Common.WebApi;
using Newtonsoft.Json.Linq;
using System;
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
        public void ItProvidesReadAccessToPropertiesThroughAnIndexer()
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

        [Fact]
        public void ItProvidesWriteAccessToPropertiesThroughAnIndexer()
        {
            dynamic model = new DynamicModel();
            model.Foo = "bar";
            model["Foo"] = "baz";

            Assert.Equal("baz", model.Foo);
        }

        [Fact]
        public void ItPreservesCaseOfThePropertyNameWhenWritingThroughAnIndexer()
        {
            dynamic model = new DynamicModel();
            model.Foo = "bar";
            model["foo"] = "baz";

            Assert.Equal("baz", model.Foo);
        }

        [Fact]
        public void ItAddsAPropertyWhenUsingTheIndexer()
        {
            dynamic model = new DynamicModel();
            model["Foo"] = "bar";

            Assert.Equal("bar", model.Foo);
        }

        [Fact]
        public void ItReturnsNullWhenGettingAPropertyThatDoesntExist()
        {
            dynamic model = new DynamicModel();
            Assert.Null(model.Absent);
        }

        [Fact]
        public void ItProvidesReadAccessToNestedPropertiesThroughAnIndexer()
        {
            dynamic model = new DynamicModel();
            model.Foo = new
            {
                Bar = "foobar"
            };

            Assert.Equal("foobar", (string) model["Foo.Bar"]);
        }

        [Fact]
        public void ItProvidesReadAccessToNestedJsonPropertiesThroughAnIndexer()
        {
            dynamic model = new DynamicModel();
            model.Json = JObject.Parse("{ foo: { bar: 'foobar' }}");

            Assert.Equal("foobar", (string) model["Json.foo.bar"]);
        }

        [Fact]
        public void ItTranslatesJValuesToBuiltInTypes()
        {
            dynamic model = new DynamicModel();
            model.Text = JValue.Parse("'foo'");
            model.Number = JValue.Parse("40.2");
            model.Boolean = JValue.Parse("true");
            model.Integer = JValue.Parse("42");
            model.Date = JValue.Parse("'2016-04-10T10:59:38Z'");

            Assert.IsType(typeof(string), model.Text);
            Assert.IsType(typeof(double), model.Number);
            Assert.IsType(typeof(bool), model.Boolean);
            Assert.IsType(typeof(int), model.Integer);
            Assert.IsType(typeof(DateTime), model.Date);

            Assert.Equal("foo", model.Text);
            Assert.Equal(40.2, model.Number);
            Assert.Equal(true, model.Boolean);
            Assert.Equal(42, model.Integer);
            Assert.Equal(new DateTime(2016, 4, 10, 10, 59, 38), model.Date);
        }
    }
}