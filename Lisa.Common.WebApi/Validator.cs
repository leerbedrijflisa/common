using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    public abstract partial class Validator
    {
        public ValidationResult Validate(DynamicModel model)
        {
            var fieldInfoContext = new FieldInfoValidationContext();
            _context = fieldInfoContext;
            _context.Validate(this);

            _context = new ModelValidationContext(model, fieldInfoContext.FieldTracker);
            _context.Validate(this);

            return Result;
        }

        public ValidationResult Validate(IEnumerable<Patch> patches, DynamicModel model)
        {
            var fieldInfoContext = new FieldInfoValidationContext();
            _context = fieldInfoContext;
            _context.Validate(this);

            _context = new PatchValidationContext(model, patches, fieldInfoContext.FieldTracker);
            _context.Validate(this);

            _context = new ModelValidationContext(model, fieldInfoContext.FieldTracker, Result);
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

        private void GatherFieldInfo()
        {
            // Run the validation with a dummy model and a dummy property. If we don't, a model
            // without properties never gets validated, its fields will never be marked optional
            // or required, and it becomes impossible to report invalid fields. Also, the
            // FieldTracker relies on the validation with the dummy property to determine whether
            // a field is marked as both required and optional.

            var resultBackup = Result;
            //Result = new ValidationResult();

            //Model = new DynamicModel();
            //Property = new KeyValuePair<string, object>(string.Empty, null);
            //ValidateModel();

            //Result = resultBackup;
        }

        private bool _allowPatch;
        private FieldTracker _fieldTracker = new FieldTracker();
        private ValidationContext _context;
    }
}