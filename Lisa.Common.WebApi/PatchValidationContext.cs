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

        public IEnumerable<Patch> ValidPatches
        {
            get { return _validPatches; }
        }

        public override void Validate(Validator validator)
        {
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

                if (isValid && !_fieldTracker.IsAllowed(patch.Field))
                {
                    var error = Error.PatchNotAllowed(patch.Field);
                    Result.Errors.Add(error);
                    isValid = false;
                }

                if (isValid)
                {
                    _validPatches.Add(patch);
                }
            }
        }

        public override void Allow(string fieldName)
        {
            // This method is empty, because the only check on patches we do right now (checking
            // if you are allowed to patch the given field) is already done by the
            // FieldInfoValidationContext. In the future, we'll probably add more checks here.
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

        private IEnumerable<Patch> _patches;
        private List<Patch> _validPatches = new List<Patch>();
        private FieldTracker _fieldTracker;
        private readonly List<string> _validPatchActions = new List<string> { "replace" };
    }
}