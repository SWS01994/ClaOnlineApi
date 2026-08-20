namespace WebAPI.Common
{
    public enum ResponseStatus { Success, ValidationError, Error }

    public static class StatusCodes
    {
        public const int Success = 200;
        public const int BadRequest = 400;
        public const int NotFound = 404;
        public const int ServerError = 500;
    }

    public static class Messages
    {
        public const string Success = "Operation completed successfully.";
        public const string ValidationFailed = "One or more validation errors occurred.";
        public const string NotFound = "The requested resource was not found.";
        public const string ServerError = "An unexpected error occurred. Please try again later.";
    }
}