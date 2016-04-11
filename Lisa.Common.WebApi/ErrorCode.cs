namespace Lisa.Common.WebApi
{
    // ErrorCode is a class instead of an enum, because projects that use Lisa.Common.WebApi should
    // be able to define their own error codes and you can't extend an enum. We don't need the
    // type-safety anyway, so a list of ints is fine.
    //
    // All fields are represented by static properties instead of const fields for two reasons.
    // 1. Const fields get baked into the assembly of the project that uses them. If we ever need
    //    to change an error code, just replacing Lisa.Common.WebApi.dll isn't enough; the project
    //    that uses Lisa.Common.WebApi needs to be rebuilt.
    // 2. Properties hide where the values come from. If we ever decide we want to, for example,
    //    load the values from a text file, we can do that without causing a breaking change.
    public static class ErrorCode
    {
        // Model validation errors
        public static int FieldMissing  { get; } = 0x0001 << 15;
        public static int ExtraField    { get; } = 0x0002 << 15;
        public static int EmptyValue    { get; } = 0x0003 << 15;
        public static int InvalidLength { get; } = 0x0004 << 15;
        public static int TooShort      { get; } = 0x0005 << 15;
        public static int TooLong       { get; } = 0x0006 << 15;
        public static int InvalidType   { get; } = 0x0008 << 15;

        // Patch validation errors
        public static int InvalidAction   { get; } = 0x0100 << 15;
        public static int InvalidField    { get; } = 0x0200 << 15;
        public static int PatchNotAllowed { get; } = 0x0300 << 15;
    }
}