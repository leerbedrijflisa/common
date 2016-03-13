using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    public abstract class Validator
    {
        public ValidationResult Validate(DynamicModel model)
        {
            Model = model;

            Property = new KeyValuePair<string, object>(string.Empty, null);
            ValidateModel();
            Result = new ValidationResult();

            foreach (var property in model.Properties)
            {
                Property = property;
                ValidateModel();

                if (!_fieldTracker.IsValid(property.Key))
                {
                    var error = new Error
                    {
                        Code = ErrorCode.InvalidField,
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
                    Values =  new
                    {
                        Field = field
                    }
                };
                Result.Errors.Add(error);
            }

            return Result;
        }

        public abstract void ValidateModel();

        protected void Required(string fieldName, params Action<string, object>[] validationFunctions)
        {
            _fieldTracker.MarkRequired(fieldName);

            if (string.Equals(Property.Key, fieldName, StringComparison.OrdinalIgnoreCase))
            {
                _fieldTracker.MarkPresent(fieldName);

                foreach (var validationFunction in validationFunctions)
                {
                    validationFunction(Property.Key, Property.Value);
                }
            }
        }

        protected void Optional(string fieldName, params Action<string, object>[] validationFunctions)
        {
            _fieldTracker.MarkOptional(fieldName);

            if (string.Equals(Property.Key, fieldName, StringComparison.OrdinalIgnoreCase))
            {
                _fieldTracker.MarkPresent(fieldName);

                foreach (var validationFunction in validationFunctions)
                {
                    validationFunction(Property.Key, Property.Value);
                }
            }
        }

        protected void NotEmpty(string fieldName, object value)
        {
            if ((value == null) ||
                (value is string) && (string.IsNullOrWhiteSpace((string) value)))
            {
                var error = new Error
                {
                    Code = ErrorCode.EmptyValue,
                    Message = $"The field '{fieldName}' should not be empty.",
                    Values = new
                    {
                        Field = fieldName
                    }
                };
                Result.Errors.Add(error);
            }
        }

        protected ValidationResult Result { get; private set; } = new ValidationResult();
        protected DynamicModel Model { get; private set; }
        protected KeyValuePair<string, object> Property { get; private set; }

        private FieldTracker _fieldTracker = new FieldTracker();
    }
}