namespace WebApiLibrary.Utilities
{
    public static class ValidationMessage
    {
        public static string Required(string fieldName) => $"{fieldName} field is required.";
        public static string MaxLength(string fieldName, int maxLength) => $"The field {fieldName} field must be a string with a maximum length of '{maxLength}'.";
        public static string PositiveValue(string fieldName) => $"{fieldName} field must be a positive value.";
        public static string MustMoreThanValue(string fieldName, int value) => $"{fieldName} field must be more than {value}.";
        public static string NotExceedValue(string fieldName, int value) => $"{fieldName} field must not exceed {value}.";
    }
}
