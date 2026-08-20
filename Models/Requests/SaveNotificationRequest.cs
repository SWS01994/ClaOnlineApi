using System;

namespace WebAPI.Models.Requests
{
    public class SaveNotificationRequest
    {
        // ---- Master: notifications ----
        public string Title { get; set; }
        public string FileNo { get; set; }
        public string NotificationDate { get; set; }
        public string Type { get; set; }
        public string NotificationNo { get; set; }
        public string Summary { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Books { get; set; }
        public string Vol { get; set; }
        public string Sections { get; set; }
        public string IssueMonth { get; set; }
        public string IssueYear { get; set; }
        public string FileName { get; set; }
        public string Citation { get; set; }
        public string Equi_Citation { get; set; }
        public string Equivalent_SCL { get; set; }
        public string Equivalent_Comp_Cas { get; set; }
        public string Equivalent_CLJ { get; set; }
        public DateTime? NotificationDate_new { get; set; }
        public bool ShowOnHome { get; set; }

        // ---- Detail: notifications_data ----
        public string Filetext { get; set; }
        public string FileHTML { get; set; }
        public int Srn { get; set; }
    }
}