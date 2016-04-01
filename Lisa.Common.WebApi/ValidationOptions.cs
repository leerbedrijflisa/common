using System;

namespace Lisa.Common.WebApi
{
    [Flags]
    public enum ValidationOptions
    {
        CaseSensitive = 1,
        CaseInsensitive = 2
    }
}