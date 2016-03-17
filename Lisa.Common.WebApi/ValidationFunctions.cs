namespace Lisa.Common.WebApi
{
    public abstract partial class Validator
    {
        protected virtual void NotEmpty(string fieldName, object value)
        {
            if ((value == null) ||
                (value is string) && (string.IsNullOrWhiteSpace((string) value)))
            {
                var error = Error.EmptyValue(fieldName);
                Result.Errors.Add(error);
            }
        }
    }
}