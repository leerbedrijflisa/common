using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    public class ModelPatcher
    {
        public void Apply(IEnumerable<Patch> patches, DynamicModel model)
        {
            foreach (var patch in patches)
            {
                model[patch.Field] = patch.Value;
            }
        }
    }
}