using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    public class ModelPatcher
    {
        public void Apply(IEnumerable<Patch> patches, DynamicModel model)
        {
            foreach (var patch in patches)
            {
                switch (patch.Action.ToLowerInvariant())
                {
                    case "replace":
                        model[patch.Field] = patch.Value;
                        break;

                    default:
                        throw new ArgumentException($"'{patch.Action}' is not a valid patch action.");
                }
            }
        }
    }
}