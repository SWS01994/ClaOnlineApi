using System;

namespace WebAPI.Models.Entities
{
    public class Query
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public string Topics { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public string Sections { get; set; }
        public int? Vol { get; set; }              
        public string IssueYear { get; set; }
        public string FileName { get; set; }
        public string Books { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool ShowOnHome { get; set; }
    }
}