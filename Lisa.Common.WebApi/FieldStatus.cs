namespace Lisa.Common.WebApi
{
    internal enum FieldStatus
    {
        Required,       // The field is known to be required, but hasn't been found yet.
        Optional,       // The field is known to be optional, but hasn't been found yet.
        Present         // The field was found, so we don't care anymore whether it's required or
                        // optional.
    }
}