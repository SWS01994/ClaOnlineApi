using System.Collections.Generic;

namespace WebAPI.Models.Responses
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }        // "Success" | "ValidationError" | "Error"
        public int StatusCode { get; set; }        // 200 / 400 / 404 / 500
        public string Message { get; set; }        // human-readable, never a raw SQL error
        public T Data { get; set; }                // payload on success, default(T) otherwise
        public List<string> Errors { get; set; }   // field-level validation messages, else null
    }
}