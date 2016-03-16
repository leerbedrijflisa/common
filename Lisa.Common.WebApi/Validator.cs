﻿using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    public abstract partial class Validator
    {
        public ValidationResult Validate(DynamicModel model)
        {
            // Make the model available to all validation functions, so that we don't have to pass
            // it around all the time. This makes derived validators easier to write and read.
            Model = model;

            // Validate the model with a dummy property. If we don't, a model without properties
            // never gets validated, its fields will never be marked optional or required, and it
            // becomes impossible to report invalid fields. Also, the FieldTracker relies on the
            // validation with the dummy property to determine whether a field is marked as both
            // required and optional.
            Property = new KeyValuePair<string, object>(string.Empty, null);
            ValidateModel();

            // Throw away any validation errors that resulted from validating the model with a
            // dummy property.
            Result = new ValidationResult();

            foreach (var property in model.Properties)
            {
                // Make the property available to all validation functions, so that we don't have
                // to pass it around all the time. This makes derived validators easier to write
                // and read.
                Property = property;
                ValidateModel();

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
                    Values =  new
                    {
                        Field = field
                    }
                };
                Result.Errors.Add(error);
            }

            return Result;
        }

        public ValidationResult Validate(IEnumerable<Patch> patches, DynamicModel model)
        {
            var result = new ValidationResult();
            List<Patch> validPatches = new List<Patch>();

            GatherFieldInfo();

            foreach (var patch in patches)
            {
                bool isValid = true;

                if (!_validPatchActions.Contains(patch.Action))
                {
                    var error = new Error
                    {
                        Code = ErrorCode.InvalidAction,
                        Message = $"'{patch.Action}' is not a valid patch action.",
                        Values = new
                        {
                            Action = patch.Action
                        }
                    };

                    result.Errors.Add(error);
                    isValid = false;
                }

                if (!_fieldTracker.Exists(patch.Field))
                {
                    var error = new Error
                    {
                        Code = ErrorCode.InvalidField,
                        Message = $"'{patch.Field}' is not a valid field.",
                        Values = new
                        {
                            Field = patch.Field
                        }
                    };

                    result.Errors.Add(error);
                    isValid = false;
                }

                if (isValid)
                {
                    Patch = patch;
                    _allowPatch = false;
                    ValidatePatch();

                    if (!_allowPatch)
                    {
                        var error = new Error
                        {
                            Code = ErrorCode.PatchNotAllowed,
                            Message = $"The field '{patch.Field}' is not patchable.",
                            Values = new
                            {
                                Field = patch.Field
                            }
                        };

                        result.Errors.Add(error);
                        isValid = false;
                    }
                }

                if (isValid)
                {
                    validPatches.Add(patch);
                }
            }

            var patcher = new ModelPatcher();
            patcher.Apply(validPatches, model);
            Validate(model);

            result.Merge(Result);
            return result;
        }

        protected abstract void ValidateModel();
        protected virtual void ValidatePatch() { }

        protected ValidationResult Result { get; private set; } = new ValidationResult();
        protected DynamicModel Model { get; private set; }
        protected KeyValuePair<string, object> Property { get; private set; }
        protected Patch Patch { get; private set; }

        protected void Required(string fieldName, params Action<string, object>[] validationFunctions)
        {
            _fieldTracker.MarkRequired(fieldName);
            ValidateField(fieldName, validationFunctions);
        }

        protected void Optional(string fieldName, params Action<string, object>[] validationFunctions)
        {
            _fieldTracker.MarkOptional(fieldName);
            ValidateField(fieldName, validationFunctions);
        }

        protected void Allow(string fieldName)
        {
            if (Patch.Field.ToLowerInvariant() == fieldName.ToLowerInvariant())
            {
                _allowPatch = true;
            }
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
            var resultBackup = Result;
            Result = new ValidationResult();

            Model = new DynamicModel();
            Property = new KeyValuePair<string, object>(string.Empty, null);
            ValidateModel();

            Result = resultBackup;
        }

        private bool _allowPatch;
        private FieldTracker _fieldTracker = new FieldTracker();
        private readonly List<string> _validPatchActions = new List<string> { "replace" };
    }
}