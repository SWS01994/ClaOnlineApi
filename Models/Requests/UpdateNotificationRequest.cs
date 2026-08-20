namespace WebAPI.Models.Requests
{
    public class UpdateNotificationRequest : SaveNotificationRequest
    {
        public int Id { get; set; }
    }
}