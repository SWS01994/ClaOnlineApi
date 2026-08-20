using System;

namespace WebAPI.Models.Requests
{
    public class NotificationSearchRequest : PagedRequest
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}