using Lisa.Common.WebApi;
using Xunit;

namespace Lisa.Common.UnitTests.WebApi
{
    public class ModelPatcherTest
    {
        [Fact]
        public void ItLeavesAModelUnchangedWhenReceivingNoPatches()
        {
            dynamic model = new DynamicModel();
            model.Category = "Nitpick";

            var patcher = new ModelPatcher();
            patcher.Apply(new Patch[0], model);

            Assert.Equal("Nitpick", model.Category);
        }

        [Fact]
        public void ItAppliesAReplacePatchToAnExistingField()
        {
            dynamic model = new DynamicModel();
            model.Category = "Nitpick";

            var patch = new Patch
            {
                Action = "replace",
                Field = "category",
                Value = "Gripe"
            };

            var patcher = new ModelPatcher();
            patcher.Apply(new Patch[] { patch }, model);

            Assert.Equal("Gripe", model.Category);
        }
    }
}
