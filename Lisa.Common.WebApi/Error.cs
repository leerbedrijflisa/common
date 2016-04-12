using System;
using System.Collections.Generic;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Values { get; set; }

        internal static Error FieldMissing(string field)
        {
            return new Error
            {
                Code = ErrorCode.FieldMissing,
                Message = $"The field '{field}' is required.",
                Values = new
                {
                    Field = field
                }
            };
        }

        internal static Error ExtraField(string field)
        {
            return new Error
            {
                Code = ErrorCode.ExtraField,
                Message = $"'{field}' is not a valid field.",
                Values = new
                {
                    Field = field
                }
            };
        }

        internal static Error EmptyValue(string field)
        {
            return new Error
            {
                Code = ErrorCode.EmptyValue,
                Message = $"The field '{field}' should not be empty.",
                Values = new
                {
                    Field = field
                }
            };
        }

        internal static Error IncorrectValue<T>(string field, T[] expected, object actual)
        {
            string values = string.Join(", ", expected);
            return new Error
            {
                Code = ErrorCode.IncorrectValue,
                Message = $"The field '{field}' has value '{actual}', but should have one of the following values: {values}.",
                Values = new
                {
                    Field = field,
                    Expected = expected,
                    Actual = actual
                }
            };
        }

        internal static Error InvalidAction(string action)
        {
            return new Error
            {
                Code = ErrorCode.InvalidAction,
                Message = $"'{action}' is not a valid patch action.",
                Values = new
                {
                    Action = action
                }
            };
        }

        internal static Error InvalidField(string field)
        {
            return new Error
            {
                Code = ErrorCode.InvalidField,
                Message = $"'{field}' is not a valid field.",
                Values = new
                {
                    Field = field
                }
            };
        }

        internal static Error PatchNotAllowed(string field)
        {
            return new Error
            {
                Code = ErrorCode.PatchNotAllowed,
                Message = $"The field '{field}' is not patchable.",
                Values = new
                {
                    Field = field
                }
            };
        }

        internal static Error InvalidLength(string field, int expected, int actual)
        {
            return new Error
            {
                Code = ErrorCode.InvalidLength,
                Message = $"The length of field '{field}' should be {expected}, but is {actual}.",
                Values = new
                {
                    Field = field,
                    Expected = expected,
                    Actual = actual
                }
            };
        }

        internal static Error InvalidLength(string field, int[] expected, int actual)
        {
            var lengths = string.Join(", ", expected);

            return new Error
            {
                Code = ErrorCode.InvalidLength,
                Message = $"The length of field '{field}' is {actual}, but should be one of the following: {lengths}.",
                Values = new
                {
                    Field = field,
                    Expected = expected,
                    Actual = actual
                }
            };
        }

        internal static Error TooShort(string field, int minimum, int actual)
        {
            return new Error
            {
                Code = ErrorCode.TooShort,
                Message = $"The length of field '{field}' should be at least {minimum}, but is {actual}.",
                Values = new
                {
                    Field = field,
                    Minimum = minimum,
                    Actual = actual
                }
            };
        }

        internal static Error TooLong(string field, int maximum, int actual)
        {
            return new Error
            {
                Code = ErrorCode.TooLong,
                Message = $"The length of field '{field}' should be at most {maximum}, but is {actual}.",
                Values = new
                {
                    Field = field,
                    Maximum = maximum,
                    Actual = actual
                }
            };
        }

        internal static Error InvalidType(string field, object value, DataTypes expected, DataTypes actual)
        {
            object expectedType = ConvertDataType(expected);
            object actualType = ConvertDataType(actual);

            
            if (expectedType is string)
            {
                return new Error
                {
                    Code = ErrorCode.InvalidType,
                    Message = $"The value of field '{field}' should be of type '{expectedType}', but is of type '{actualType}'.",
                    Values = new
                    {
                        Field = field,
                        Expected = expectedType,
                        Actual = actualType,
                        Value = value
                    }
                };
            }
            else
            {
                string expectedTypes;
                expectedTypes = string.Join(", ", expectedType);

                return new Error
                {
                    Code = ErrorCode.InvalidType,
                    Message = $"The value of field '{field}' is of type '{actualType}', but should be of one of the following types: {expectedTypes}.",
                    Values = new
                    {
                        Field = field,
                        Expected = expectedType,
                        Actual = actualType,
                        Value = value
                    }
                };
            }
        }

        internal static Error NoMatch(string fieldName, object value, string pattern)
        {
            return new Error
            {
                Code = ErrorCode.NoMatch,
                Message = $"The value of field '{fieldName}' should match the pattern '{pattern}'.",
                Values = new
                {
                    Field = fieldName,
                    Value = value,
                    Pattern = pattern
                }
            };
        }

        private static object ConvertDataType(DataTypes type)
        {
            var types = new List<string>();

            if ((type & DataTypes.String) != 0)
            {
                types.Add("string");
            }

            if ((type & DataTypes.Number) != 0)
            {
                types.Add("number");
            }

            if ((type & DataTypes.Boolean) != 0)
            {
                types.Add("boolean");
            }

            if ((type & DataTypes.Array) != 0)
            {
                types.Add("array");
            }

            if ((type & DataTypes.Object) != 0)
            {
                types.Add("object");
            }

            if (types.Count == 1)
            {
                return types[0];
            }
            else
            {
                return types.ToArray();
            }
        }
    }
}