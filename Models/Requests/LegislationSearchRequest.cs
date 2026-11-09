using System;

namespace WebAPI.Models.Requests
{
    public class LegislationSearchRequest : PagedRequest
    {
        public string Category { get; set; }
        public string Subject { get; set; }
        public int? IssueYear { get; set; }
        public bool? ShowOnHome { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}