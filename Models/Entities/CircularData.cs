namespace WebAPI.Models.Entities
{
    public class CircularData
    {
        public int ID { get; set; }
        public int Notification_ID { get; set; }   // link column is Notification_ID (not Circular_ID)
        public string FileName { get; set; }
        public string Filetext { get; set; }
        public string FileHTML { get; set; }
        public int Srn { get; set; }   // maps to 'srn'
    }
}