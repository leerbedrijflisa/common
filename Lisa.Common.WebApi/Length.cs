using System;
using System.Collections;
using System.Linq;

namespace Lisa.Common.WebApi
{
    public partial class Validator
    {
        protected virtual Action<string, object> Length(params int[] lengths)
        {
            return (fieldName, value) =>
            {
                if (value == null || !((value is ICollection) || (value is string)))
                {
                    return;
                }

                int actual = value is ICollection ? ((ICollection) value).Count : ((string) value).Length;
                if (!lengths.Any(expected => expected == actual))
                {
                    var error = Error.InvalidLength(fieldName, lengths, actual);
                    Result.Errors.Add(error);
                }
            };
        }

        protected virtual Action<string, object> Length(int length)
        {
            return (fieldName, value) =>
            {
                if (value == null || !((value is ICollection) || (value is string)))
                {
                    return;
                }

                int actual = value is ICollection ? ((ICollection) value).Count : ((string) value).Length;
                if (actual != length)
                {
                    var error = Error.InvalidLength(fieldName, length, actual);
                    Result.Errors.Add(error);
                }
            };
        }

        protected virtual Action<string, object> MinLength(int minimum)
        {
            return (fieldName, value) =>
            {
                if (value == null || !((value is ICollection) || (value is string)))
                {
                    return;
                }

                int actual = value is ICollection ? ((ICollection) value).Count : ((string) value).Length;
                if (actual < minimum)
                {
                    var error = Error.TooShort(fieldName, minimum, actual);
                    Result.Errors.Add(error);
                }
            };
        }

        protected virtual Action<string, object> MaxLength(int maximum)
        {
            return (fieldName, value) =>
            {
                if (value == null || !((value is ICollection) || (value is string)))
                {
                    return;
                }

                int actual = value is ICollection ? ((ICollection) value).Count : ((string) value).Length;
                if (actual > maximum)
                {
                    var error = Error.TooLong(fieldName, maximum, actual);
                    Result.Errors.Add(error);
                }
            };
        }
    }
}