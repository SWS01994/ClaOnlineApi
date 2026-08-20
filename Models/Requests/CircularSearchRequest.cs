using System;

namespace WebAPI.Models.Requests
{
    public class CircularSearchRequest : PagedRequest
    {
        //public string Category { get; set; }
        //public string IssueYear { get; set; }
        //public bool? ShowOnHome { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}