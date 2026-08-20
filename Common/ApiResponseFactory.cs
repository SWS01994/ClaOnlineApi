using System.Collections.Generic;
using WebAPI.Models.Responses;

namespace WebAPI.Common
{
    public static class ApiResponseFactory
    {
        public static ApiResponse<T> Success<T>(T data)
        {
            return Success<T>(data, Messages.Success);
        }

        public static ApiResponse<T> Success<T>(T data, string message)
        {
            ApiResponse<T> r = new ApiResponse<T>();
            r.Status = ResponseStatus.Success.ToString();
            r.StatusCode = StatusCodes.Success;
            r.Message = message;
            r.Data = data;
            r.Errors = null;
            return r;
        }

        public static ApiResponse<T> ValidationError<T>(List<string> errors)
        {
            ApiResponse<T> r = new ApiResponse<T>();
            r.Status = ResponseStatus.ValidationError.ToString();
            r.StatusCode = StatusCodes.BadRequest;
            r.Message = Messages.ValidationFailed;
            r.Data = default(T);
            r.Errors = errors;
            return r;
        }

        public static ApiResponse<T> NotFound<T>(string message)
        {
            ApiResponse<T> r = new ApiResponse<T>();
            r.Status = ResponseStatus.Error.ToString();
            r.StatusCode = StatusCodes.NotFound;
            r.Message = message;
            r.Data = default(T);
            r.Errors = null;
            return r;
        }

        public static ApiResponse<T> Error<T>(string message)
        {
            ApiResponse<T> r = new ApiResponse<T>();
            r.Status = ResponseStatus.Error.ToString();
            r.StatusCode = StatusCodes.ServerError;
            r.Message = message;
            r.Data = default(T);
            r.Errors = null;
            return r;
        }
    }
}