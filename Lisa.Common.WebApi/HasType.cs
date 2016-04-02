using System;
using System.Collections;

namespace Lisa.Common.WebApi
{
    public partial class Validator
    {
        protected virtual Action<string, object> HasType(DataTypes accepted)
        {
            return (fieldName, value) =>
            {
                if (value == null)
                {
                    return;
                }

                DataTypes actual = GetDataType(value);

                if ((actual & accepted) == 0)
                {
                    var error = Error.InvalidType(fieldName, value, accepted, actual);
                    Result.Errors.Add(error);
                }
            };
        }

        protected virtual Action<string, object> IsArray(DataTypes accepted)
        {
            return (fieldName, value) =>
            {
                DataTypes actual = GetDataType(value);
                if ((actual & DataTypes.Array) == 0)
                {
                    var error = Error.InvalidType(fieldName, value, accepted, actual);
                    Result.Errors.Add(error);
                    return;
                }

                var hasType = HasType(accepted);
                foreach (var element in (IEnumerable) value)
                {
                    hasType(fieldName, element);
                }
            };
        }

        private DataTypes GetDataType(object value)
        {
            if (value is string)
            {
                return DataTypes.String;
            }

            if (value is int || value is float || value is double)
            {
                return DataTypes.Number;
            }

            if (value is bool)
            {
                return DataTypes.Boolean;
            }

            if (value is IList)
            {
                return DataTypes.Array;
            }

            return DataTypes.Unknown;
        }
    }
}