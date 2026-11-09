namespace WebAPI.Models.Requests
{
    public class SaveLegislationRequest
    {
        // ---- Master: Legislation ----
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
        public string LegislationNo { get; set; }
        public int? IssueYear { get; set; }
        public string TOCname { get; set; }
        public string Filename { get; set; }
        public bool ShowOnHome { get; set; }
        public int? FileID { get; set; }

        // ---- Detail: legislation_data ----
        public string Filetext { get; set; }
        public string FileHTML { get; set; }
        public int Srn { get; set; }
    }
}