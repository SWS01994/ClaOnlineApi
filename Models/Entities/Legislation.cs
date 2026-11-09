using System;

namespace WebAPI.Models.Entities
{
    public class Legislation
    {
        public int id { get; set; }
        public string BookMark { get; set; }
        public string Title { get; set; }
        public string Headings { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Type { get; set; }
        public string TitleNo { get; set; }
        public string ChapterHeading { get; set; }   
        public string Books { get; set; }
        public string Vol { get; set; }
        public string Sections { get; set; }
        public string LegislationNo { get; set; }     // DB column is 'Legislation' — renamed to avoid class-name clash
        public int? IssueYear { get; set; }            // int here, unlike other tables
        public string TOCname { get; set; }
        public string Filename { get; set; }
        public DateTime? InsertedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public bool ShowOnHome { get; set; }
        public int? FileID { get; set; }
    }
}