using System;
using System.Text.RegularExpressions;

namespace Lisa.Common.WebApi
{
    public partial class Validator
    {
        protected virtual Action<string, object> Match(string pattern, RegexOptions options = RegexOptions.None)
        {
            return (fieldName, value) =>
            {
                if (value == null)
                {
                    return;
                }

                if (!Regex.IsMatch(value.ToString(), pattern, options))
                {
                    var error = Error.NoMatch(fieldName, value, pattern);
                    Result.Errors.Add(error);
                }
            };
        }
    }
}