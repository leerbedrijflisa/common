using System;
using System.Collections;

namespace Lisa.Common.WebApi
{
    public partial class Validator
    {
        protected virtual Action<string, object> Length(int length)
        {
            return (fieldName, value) =>
            {
                if (value == null || !(value is ICollection))
                {
                    return;
                }

                var collection = (ICollection) value;
                if (collection.Count != length)
                {
                    var error = Error.InvalidLength(fieldName, length, collection.Count);
                    Result.Errors.Add(error);
                }
            };
        }

        protected virtual Action<string, object> MinLength(int minimum)
        {
            return (fieldName, value) =>
            {
                if (value == null || !(value is ICollection))
                {
                    return;
                }

                var collection = (ICollection) value;
                if (collection.Count < minimum)
                {
                    var error = Error.TooShort(fieldName, minimum, collection.Count);
                    Result.Errors.Add(error);
                }
            };
        }

        protected virtual Action<string, object> MaxLength(int maximum)
        {
            return (fieldName, value) =>
            {
                if (value == null || !(value is ICollection))
                {
                    return;
                }

                var collection = (ICollection) value;
                if (collection.Count > maximum)
                {
                    var error = Error.TooLong(fieldName, maximum, collection.Count);
                    Result.Errors.Add(error);
                }
            };
        }
    }
}