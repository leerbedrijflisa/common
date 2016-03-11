using Lisa.Common.WebApi;
using Microsoft.CSharp.RuntimeBinder;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
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
    }
}