using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public abstract class Validator
    {
        public ValidationResult Validate(DynamicModel model)
        {
            Model = model;
            _fields = new Dictionary<string, FieldStatus>();

            Property = new KeyValuePair<string, object>(string.Empty, null);
            ValidateModel();
            Result = new ValidationResult();

            foreach (var property in model.Properties)
            {
                Property = property;
                ValidateModel();

                var field = _fields.SingleOrDefault(f => f.Key.ToLowerInvariant() == property.Key.ToLowerInvariant());
                if (field.Key == null)
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

            foreach (var field in _fields)
            {
                if (field.Value == FieldStatus.Required)
                {
                    var error = new Error
                    {
                        Code = ErrorCode.FieldMissing,
                        Message = $"The field '{field.Key}' is required.",
                        Values =  new
                        {
                            Field = field.Key
                        }
                    };
                    Result.Errors.Add(error);
                }
            }

            return Result;
        }

        public abstract void ValidateModel();

        protected void Required(string fieldName, params Action<string, object>[] validationFunctions)
        {
            var name = fieldName.ToLowerInvariant();
            var field = _fields.SingleOrDefault(f => f.Key.ToLowerInvariant() == name);
            if (field.Key == null)
            {
                _fields[fieldName] = FieldStatus.Required;
            }

            if (Property.Key.ToLowerInvariant() == name)
            {
                _fields[fieldName] = FieldStatus.Present;

                foreach (var validationFunction in validationFunctions)
                {
                    validationFunction(Property.Key, Property.Value);
                }
            }
        }

        protected void Optional(string fieldName, params Action<string, object>[] validationFunctions)
        {
            var name = fieldName.ToLowerInvariant();
            var field = _fields.SingleOrDefault(f => f.Key.ToLowerInvariant() == name);
            if (field.Key == null)
            {
                _fields[fieldName] = FieldStatus.Optional;
            }

            if (Property.Key.ToLowerInvariant() == name)
            {
                _fields[fieldName] = FieldStatus.Present;

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

        private Dictionary<string, FieldStatus> _fields;
    }
}