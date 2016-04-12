using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

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
                foreach (var nestedProperty in GetNestedProperties(property))
                {
                    Property = nestedProperty;
                    validator.ValidateModel();

                    if (!_fieldTracker.Exists(nestedProperty.Key))
                    {
                        var error = Error.ExtraField(nestedProperty.Key);
                        Result.Errors.Add(error);
                    }
                }
            }

            foreach (var field in _fieldTracker.MissingFields)
            {
                var error = Error.FieldMissing(field);
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

        public override void Ignore(string fieldName)
        {
            // Intentionally empty. What did you expect from a function called Ignore?
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

        private IEnumerable<KeyValuePair<string, object>> GetNestedProperties(KeyValuePair<string, object> property)
        {
            if (property.Value == null)
            {
                yield return property;
            }
            else if (property.Value is ICollection) // NOTE: IEnumerable doesn't work because string is also IEnumerable
            {
                yield return property;

                foreach (var item in (IEnumerable) property.Value)
                {
                    if (!IsNestedType(item))
                    {
                        continue;
                    }

                    var nestedProperties = GetNestedProperties(new KeyValuePair<string, object>(property.Key, item));
                    foreach (var nestedProperty in nestedProperties)
                    {
                        yield return nestedProperty;
                    }
                }
            }
            else if (property.Value is IDynamicMetaObjectProvider)
            {
                var expression = Expression.Variable(property.Value.GetType());
                var nestedPropertyNames = ((IDynamicMetaObjectProvider) property.Value).GetMetaObject(expression).GetDynamicMemberNames();
                if (nestedPropertyNames.Count() == 0)
                {
                    yield return property;
                    yield break;
                }

                foreach (string nestedPropertyName in nestedPropertyNames)
                {
                    // Code adapted from http://stackoverflow.com/a/7108263
                    var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(CSharpBinderFlags.None, nestedPropertyName, property.Value.GetType(), new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                    var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
                    object value = callsite.Target(callsite, property.Value);
                    string name = $"{property.Key}.{nestedPropertyName}";

                    var nestedProperties = GetNestedProperties(new KeyValuePair<string, object>(name, value));
                    foreach (var nestedProperty in nestedProperties)
                    {
                        yield return nestedProperty;
                    }
                }
            }
            else if (IsNestedType(property.Value))
            {
                yield return property;

                var nestedPropertiesInfos = property.Value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo nestedPropertyInfo in nestedPropertiesInfos)
                {
                    object value = nestedPropertyInfo.GetValue(property.Value);
                    string name = $"{property.Key}.{nestedPropertyInfo.Name}";

                    var nestedProperties = GetNestedProperties(new KeyValuePair<string, object>(name, value));
                    foreach (var nestedProperty in nestedProperties)
                    {
                        yield return nestedProperty;
                    }
                }
            }
            else
            {
                yield return property;
            }
        }

        private bool IsNestedType(object value)
        {
            // Code adapted from http://stackoverflow.com/a/2483054
            Type type = value?.GetType();
            return type != null
                && ((type.Name.Contains("AnonymousType") && type.Name.StartsWith("<>"))
                || (value is DynamicModel));
        }

        private FieldTracker _fieldTracker;
    }
}
