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

        protected virtual Action<string, object> OneOf(params double[] values)
        {
            return (fieldName, value) =>
            {
                if (value == null)
                {
                    return;
                }

                double convertedValue;
                if (value is double)
                {
                    convertedValue = (double) value;
                }
                else if (value is int)
                {
                    convertedValue = (int) value;
                }
                else if (value is float)
                {
                    convertedValue = (float) value;
                }
                else if (value is decimal)
                {
                    convertedValue = decimal.ToDouble((decimal) value);
                }
                else
                {
                    var error = Error.IncorrectValue(fieldName, values);
                    Result.Errors.Add(error);
                    return;
                }

                double variance = Math.Pow(10, -8);
                if (!values.Any(v => Math.Abs(convertedValue - v) < variance))
                {
                    var error = Error.IncorrectValue(fieldName, values);
                    Result.Errors.Add(error);
                }
            };
        }
    }
}