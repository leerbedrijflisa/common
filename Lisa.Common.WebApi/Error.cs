using System;

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

        internal static Error InvalidType(string field, DataType accepted, DataType actual)
        {
            string acceptedType = DataTypeToString(accepted);
            string actualType = DataTypeToString(actual);

            return new Error
            {
                Code = ErrorCode.InvalidType,
                Message = $"The value of field '{field}' should be of type '{acceptedType}', but is of type '{actualType}'.",
                Values = new
                {
                    Field = field,
                    Accepted = acceptedType,
                    Actual = actualType
                }
            };
        }

        private static string DataTypeToString(DataType type)
        {
            switch (type)
            {
                case DataType.String:
                    return "string";

                case DataType.Number:
                    return "number";

                case DataType.Boolean:
                    return "boolean";

                case DataType.Array:
                    return "array";

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}