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

        internal static Error IncorrectValue<T>(string field, T[] allowed)
        {
            string values = string.Join(", ", allowed);
            return new Error
            {
                Code = ErrorCode.IncorrectValue,
                Message = $"The field '{field}' should have one of the following values: {values}.",
                Values = new
                {
                    Allowed = allowed
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
    }
}