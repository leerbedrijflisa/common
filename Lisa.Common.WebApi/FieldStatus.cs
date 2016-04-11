using System;

namespace Lisa.Common.WebApi
{
    [Flags]
    internal enum FieldStatus
    {
        Required = 1,
        Optional = 2,
        Present = 4,
        Allowed = 8,
        Ignore = 16,
        SubField = 32
    }
}