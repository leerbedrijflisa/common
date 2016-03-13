using System.Collections.Generic;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public abstract class Validator
    {
        public ValidationResult Validate(DynamicModel model)
        {
            Model = model;
            _fields = new Dictionary<string, bool>();

            foreach (var property in model.Properties)
            {
                Property = property;
                ValidateModel();
            }

            foreach (var field in _fields)
            {
                if (field.Value == false)
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

        public void Required(string fieldName)
        {
            var name = fieldName.ToLowerInvariant();
            var field = _fields.SingleOrDefault(f => f.Key.ToLowerInvariant() == name);
            if (field.Key == null)
            {
                _fields[fieldName] = false;
            }

            if (Property.Key.ToLowerInvariant() == name)
            {
                _fields[fieldName] = true;
            }
        }

        protected ValidationResult Result { get; private set; } = new ValidationResult();
        protected DynamicModel Model { get; private set; }
        protected KeyValuePair<string, object> Property { get; private set; }

        private Dictionary<string, bool> _fields;
    }
}