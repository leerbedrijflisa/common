using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    internal class PatchValidationContext : ValidationContext
    {
        public PatchValidationContext(DynamicModel model, IEnumerable<Patch> patches, FieldTracker fieldTracker)
        {
            Model = model;
            Result = new ValidationResult();
            _patches = patches;
            _fieldTracker = fieldTracker;
        }

        public override void Validate(Validator validator)
        {
            List<Patch> validPatches = new List<Patch>();

            foreach (var patch in _patches)
            {
                bool isValid = true;

                if (!_validPatchActions.Contains(patch.Action))
                {
                    var error = Error.InvalidAction(patch.Action);
                    Result.Errors.Add(error);
                    isValid = false;
                }

                if (!_fieldTracker.Exists(patch.Field))
                {
                    var error = Error.InvalidField(patch.Field);
                    Result.Errors.Add(error);
                    isValid = false;
                }

                if (isValid)
                {
                    Patch = patch;
                    _allowPatch = false;
                    validator.ValidatePatch();

                    if (!_allowPatch)
                    {
                        var error = Error.PatchNotAllowed(patch.Field);
                        Result.Errors.Add(error);
                        isValid = false;
                    }
                }

                if (isValid)
                {
                    validPatches.Add(patch);
                }
            }

            var patcher = new ModelPatcher();
            patcher.Apply(validPatches, Model);
        }

        public override void Allow(string fieldName)
        {
            if (string.Equals(Patch.Field, fieldName, StringComparison.OrdinalIgnoreCase))
            {
                _allowPatch = true;
            }
        }

        public override void Required(string fieldName, params Action<string, object>[] validationFunctions)
        {
            throw new InvalidOperationException("Calling Required() is not valid inside ValidatePatch()");
        }

        public override void Optional(string fieldName, params Action<string, object>[] validationFunctions)
        {
            throw new InvalidOperationException("Calling Optional() is not valid inside ValidatePatch()");
        }

        private void ValidateField(string fieldName, Action<string, object>[] validationFunctions)
        {
            if (string.Equals(Property.Key, fieldName, StringComparison.OrdinalIgnoreCase))
            {
                _fieldTracker.MarkPresent(fieldName);

                foreach (var validationFunction in validationFunctions)
                {
                    validationFunction(Property.Key, Property.Value);
                }
            }
        }

        private bool _allowPatch;
        private IEnumerable<Patch> _patches;
        private FieldTracker _fieldTracker;
        private readonly List<string> _validPatchActions = new List<string> { "replace" };
    }
}