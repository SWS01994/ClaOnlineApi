using WebAPI.Models.Entities;

namespace WebAPI.Models.Responses
{
    public class NotificationDetailResponse
    {
        public Notification Notification { get; set; }
        public NotificationData Content { get; set; }
    }
}