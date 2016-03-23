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
    }
}