using System;

namespace Lisa.Common.WebApi
{
    internal class ModelValidationContext : ValidationContext
    {
        public ModelValidationContext(DynamicModel model, FieldTracker fieldTracker)
        {
            Model = model;
            Result = new ValidationResult();
            _fieldTracker = fieldTracker;
        }

        public ModelValidationContext(DynamicModel model, FieldTracker fieldTracker, ValidationResult result)
        {
            Model = model;
            Result = result;
            _fieldTracker = fieldTracker;
        }

        public override Patch Patch
        {
            get { throw new InvalidOperationException("Accessing Patch is not valid inside ValidateModel()."); }
        }

        public override void Validate(Validator validator)
        {
            foreach (var property in Model.Properties)
            {
                Property = property;
                validator.ValidateModel();

                if (!_fieldTracker.Exists(property.Key))
                {
                    var error = new Error
                    {
                        Code = ErrorCode.ExtraField,
                        Message = $"'{property.Key}' is not a valid field.",
                        Values = new
                        {
                            Field = property.Key
                        }
                    };
                    Result.Errors.Add(error);
                }
            }

            foreach (var field in _fieldTracker.MissingFields)
            {
                var error = new Error
                {
                    Code = ErrorCode.FieldMissing,
                    Message = $"The field '{field}' is required.",
                    Values = new
                    {
                        Field = field
                    }
                };
                Result.Errors.Add(error);
            }
        }

        public override void Required(string fieldName, params Action<string, object>[] validationFunctions)
        {
            ValidateField(fieldName, validationFunctions);
        }

        public override void Optional(string fieldName, params Action<string, object>[] validationFunctions)
        {
            ValidateField(fieldName, validationFunctions);
        }

        public override void Allow(string fieldName)
        {
            throw new InvalidOperationException("Calling Allow() is not valid inside ValidateModel().");
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

        private FieldTracker _fieldTracker;
    }
}
