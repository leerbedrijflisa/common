using System;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public partial class Validator
    {
        protected virtual Action<string, object> OneOf(params string[] values)
        {
            return (fieldName, value) =>
            {
                if (value != null && !values.Contains(value))
                {
                    var error = Error.IncorrectValue(fieldName, values);
                    Result.Errors.Add(error);
                }
            };
        }
    }
}