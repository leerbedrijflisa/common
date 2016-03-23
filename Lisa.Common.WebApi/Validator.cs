using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    // The validator works in different contexts, because each context requires different behavior
    // when validating. The FieldInfoContext is only interested in gathering information about
    // whether a field is optional or required, so it doesn't need to run all the validation
    // functions. The ModelValidationContext runs the validation functions on the model, but
    // doesn't do anything with patches. The PatchValidationContext runs validation functions on
    // the patches and on the (patched) model.
    public abstract partial class Validator
    {
        public ValidationResult Validate(DynamicModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            // Gather information about optional and required fields.
            var fieldInfoContext = new FieldInfoValidationContext();
            _context = fieldInfoContext;
            _context.Validate(this);

            // Run the validation functions for the model.
            _context = new ModelValidationContext(model, fieldInfoContext.FieldTracker);
            _context.Validate(this);

            return Result;
        }

        public ValidationResult Validate(IEnumerable<Patch> patches, DynamicModel model)
        {
            if (patches == null)
            {
                throw new ArgumentNullException("patches");
            }

            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            // Gather information about which fields can be patched.
            var fieldInfoContext = new FieldInfoValidationContext();
            _context = fieldInfoContext;
            _context.Validate(this);

            // Run the validation functions for the patches.
            var patchValidationContext = new PatchValidationContext(model, patches, fieldInfoContext.FieldTracker);
            _context = patchValidationContext;
            _context.Validate(this);

            // Create a patched copy of the model in memory, so we can test the model for
            // validation errors after patching. We do this on a copy of the model, because we
            // don't want the validator to also be a patcher.
            var copy = model.Copy();
            var patcher = new ModelPatcher();
            patcher.Apply(patchValidationContext.ValidPatches, copy);

            // Runs the validation functios on the patched model.
            _context = new ModelValidationContext(copy, fieldInfoContext.FieldTracker, Result);
            _context.Validate(this);

            return Result;
        }

        protected internal abstract void ValidateModel();
        protected internal virtual void ValidatePatch() { }

        protected ValidationResult Result
        {
            get { return _context.Result; }
        }

        protected DynamicModel Model
        {
            get { return _context.Model; }
        }

        protected KeyValuePair<string, object> Property
        {
            get { return _context.Property; }
        }

        protected Patch Patch
        {
            get { return _context.Patch; }
        }

        protected void Required(string fieldName, params Action<string, object>[] validationFunctions)
        {
            _context.Required(fieldName, validationFunctions);
        }

        protected void Optional(string fieldName, params Action<string, object>[] validationFunctions)
        {
            _context.Optional(fieldName, validationFunctions);
        }

        protected void Ignore(string fieldName)
        {
            _context.Ignore(fieldName);
        }

        protected void Allow(string fieldName)
        {
            _context.Allow(fieldName);
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

        private FieldTracker _fieldTracker = new FieldTracker();
        private ValidationContext _context;
    }
}