using System;
using System.Collections;

namespace Lisa.Common.WebApi
{
    public partial class Validator
    {
        protected virtual Action<string, object> HasType(DataType accepted)
        {
            return (fieldName, value) =>
            {
                DataType actual = GetDataType(value);

                if (accepted != actual)
                {
                    var error = Error.InvalidType(fieldName, accepted, actual);
                    Result.Errors.Add(error);
                }
            };
        }

        private DataType GetDataType(object value)
        {
            if (value is string)
            {
                return DataType.String;
            }

            if (value is int || value is float || value is double)
            {
                return DataType.Number;
            }

            if (value is bool)
            {
                return DataType.Boolean;
            }

            if (value is IList)
            {
                return DataType.Array;
            }

            return DataType.Unknown;
        }
    }
}