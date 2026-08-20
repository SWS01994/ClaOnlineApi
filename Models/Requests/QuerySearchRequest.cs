using System;

namespace WebAPI.Models.Requests
{
    public class QuerySearchRequest : PagedRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}