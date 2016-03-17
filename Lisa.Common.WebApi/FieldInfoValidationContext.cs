using System;

namespace Lisa.Common.WebApi
{
    internal class FieldInfoValidationContext : ValidationContext
    {
        public FieldInfoValidationContext()
        {
            Result = new ValidationResult();
        }

        public FieldTracker FieldTracker { get; } = new FieldTracker();

        public override void Validate(Validator validator)
        {
            validator.ValidateModel();
        }

        public override void Required(string fieldName, params Action<string, object>[] validationFunctions)
        {
            FieldTracker.MarkRequired(fieldName);
        }

        public override void Optional(string fieldName, params Action<string, object>[] validationFunctions)
        {
            FieldTracker.MarkOptional(fieldName);
        }

        public override void Allow(string fieldName)
        {
            //FieldTracker.MarkAllowed(fieldName);
        }
    }
}