using System;

namespace WebAPI.Models.Entities
{
    public class Notification
    {
        public int Id { get; set; }                    // PK casing is 'Id'
        public string Title { get; set; }
        public string FileNo { get; set; }
        public string NotificationDate { get; set; }    // legacy nvarchar date
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
        public DateTime? InsertedDate { get; set; }
        public DateTime? NotificationDate_new { get; set; }   // newer datetime date
        public DateTime? UpdateDate { get; set; }
        public bool ShowOnHome { get; set; }
    }
}