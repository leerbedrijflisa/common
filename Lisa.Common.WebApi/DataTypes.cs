using System;

namespace Lisa.Common.WebApi
{
    [Flags]
    public enum DataTypes
    {
        Unknown = 0,
        String = 1,
        Number = 2,
        Boolean = 4,
        Array = 8,
        Object = 16
    }
}