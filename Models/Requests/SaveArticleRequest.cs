namespace WebAPI.Models.Requests
{
    public class SaveArticleRequest
    {
        // ---- Master: Articles ----
        public string Title { get; set; }
        public string Author { get; set; }
        public string Summary { get; set; }
        public string Category { get; set; }
        public string Subject { get; set; }
        public string Vol { get; set; }
        public string Topics { get; set; }
        public string Books { get; set; }
        public string Sections { get; set; }
        public string IssueMonth { get; set; }
        public string IssueYear { get; set; }
        public string FileName { get; set; }
        public bool ShowOnHome { get; set; }
        public int OrderNumber { get; set; }

        // ---- Detail: Articles_data ----
        public string Filetext { get; set; }
        public string FileHTML { get; set; }
        public int Srn { get; set; }
    }
}