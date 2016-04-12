using System;
using System.Collections;

namespace Lisa.Common.WebApi
{
    public partial class Validator
    {
        protected virtual Action<string, object> TypeOf(DataTypes expected)
        {
            return (fieldName, value) =>
            {
                if (value == null)
                {
                    return;
                }

                DataTypes actual = GetDataType(value);

                if ((actual & expected) == 0)
                {
                    var error = Error.InvalidType(fieldName, value, expected, actual);
                    Result.Errors.Add(error);
                }
            };
        }

        protected virtual Action<string, object> IsArray(DataTypes expected)
        {
            return (fieldName, value) =>
            {
                DataTypes actual = GetDataType(value);
                if ((actual & DataTypes.Array) == 0)
                {
                    var error = Error.InvalidType(fieldName, value, expected, actual);
                    Result.Errors.Add(error);
                    return;
                }

                var hasType = TypeOf(expected);
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

            return DataTypes.Object;
        }
    }
}