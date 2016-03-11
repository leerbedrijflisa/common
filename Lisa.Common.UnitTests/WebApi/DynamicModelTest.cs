using Lisa.Common.WebApi;
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
    }
}