using System;
using System.Collections.Generic;

namespace Lisa.Common.WebApi
{
    internal abstract class ValidationContext
    {
        public virtual ValidationResult Result { get; protected set; }
        public virtual DynamicModel Model { get; protected set; }
        public virtual KeyValuePair<string, object> Property { get; protected internal set; }
        public virtual Patch Patch { get; protected internal set; }

        public abstract void Validate(Validator validator);
        public abstract void Required(string fieldName, params Action<string, object>[] validationFunctions);
        public abstract void Optional(string fieldName, params Action<string, object>[] validationFunctions);
        public abstract void Allow(string fieldName);
    }
}