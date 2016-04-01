using System;
using System.Collections;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public partial class Validator
    {
        protected virtual Action<string, object> OneOf(params string[] accepted)
        {
            return (fieldName, value) =>
            {
                if (value == null)
                {
                    return;
                }

                IList values = value as IList;
                if (values == null)
                {
                    values = new[] { value };
                }

                foreach (var v in values)
                {
                    if (!accepted.Contains(v))
                    {
                        var error = Error.IncorrectValue(fieldName, accepted, v);
                        Result.Errors.Add(error);
                    }
                }
            };
        }

        protected virtual Action<string, object> OneOf(params double[] accepted)
        {
            return (fieldName, value) =>
            {
                if (value == null)
                {
                    return;
                }

                IList values = value as IList;
                if (values == null)
                {
                    values = new[] { value };
                }

                foreach (var v in values)
                {
                    double convertedValue;
                    if (v is double)
                    {
                        convertedValue = (double) v;
                    }
                    else if (v is int)
                    {
                        convertedValue = (int) v;
                    }
                    else if (v is float)
                    {
                        convertedValue = (float) v;
                    }
                    else if (v is decimal)
                    {
                        convertedValue = decimal.ToDouble((decimal) v);
                    }
                    else
                    {
                        var error = Error.IncorrectValue(fieldName, accepted, v);
                        Result.Errors.Add(error);
                        return;
                    }

                    double variance = Math.Pow(10, -8);
                    if (!accepted.Any(val => Math.Abs(convertedValue - val) < variance))
                    {
                        var error = Error.IncorrectValue(fieldName, accepted, convertedValue);
                        Result.Errors.Add(error);
                    }
                }
            };
        }
    }
}