using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    public abstract partial class Validator
    {
        public ValidationResult Validate(DynamicModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            var fieldInfoContext = new FieldInfoValidationContext();
            _context = fieldInfoContext;
            _context.Validate(this);

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

            var fieldInfoContext = new FieldInfoValidationContext();
            _context = fieldInfoContext;
            _context.Validate(this);

            var patchValidationContext = new PatchValidationContext(model, patches, fieldInfoContext.FieldTracker);
            _context = patchValidationContext;
            _context.Validate(this);

            var copy = model.Copy();
            var patcher = new ModelPatcher();
            patcher.Apply(patchValidationContext.ValidPatches, copy);

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