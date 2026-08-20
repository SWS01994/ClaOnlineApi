namespace WebAPI.Models.Requests
{
    public class SaveQueryRequest
    {
        // ---- Master: Query ----
        public string Subject { get; set; }
        public string Topics { get; set; }
        public string Summary { get; set; }
        public string Title { get; set; }
        public string Sections { get; set; }
        public int? Vol { get; set; }
        public string IssueYear { get; set; }
        public string FileName { get; set; }
        public string Books { get; set; }
        public bool ShowOnHome { get; set; }

        // ---- Detail: query_data ----
        public string Filetext { get; set; }
        public string FileHTML { get; set; }
        public int Srn { get; set; }
    }
}