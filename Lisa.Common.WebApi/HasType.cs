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
                    var error = Error.InvalidType(fieldName, accepted, actual);
                    Result.Errors.Add(error);
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