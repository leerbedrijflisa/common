using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    internal class FieldInfoValidationContext : ValidationContext
    {
        public FieldInfoValidationContext()
        {
        }

        public FieldTracker FieldTracker { get; } = new FieldTracker();

        public override DynamicModel Model
        {
            get { throw new InvalidOperationException("You cannot access Model outside of validation functions."); }
        }

        public override KeyValuePair<string, object> Property
        {
            get { throw new InvalidOperationException("You cannot access Property outside of validation functions."); }
        }

        public override ValidationResult Result
        {
            get { throw new InvalidOperationException("You cannot access Result outside of validation functions."); }
        }

        public override Patch Patch
        {
            get { throw new InvalidOperationException("You cannot access Patch outside of validation functions."); }
        }

        public override void Validate(Validator validator)
        {
            validator.ValidateModel();
            validator.ValidatePatch();
        }

        public override void Required(string fieldName, params Action<string, object>[] validationFunctions)
        {
            FieldTracker.MarkRequired(fieldName);
        }

        public override void Optional(string fieldName, params Action<string, object>[] validationFunctions)
        {
            FieldTracker.MarkOptional(fieldName);
        }

        public override void Ignore(string fieldName)
        {
            FieldTracker.MarkIgnored(fieldName);
        }

        public override void Allow(string fieldName)
        {
            FieldTracker.MarkAllowed(fieldName);
        }
    }
}