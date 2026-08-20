namespace WebAPI.Models.Entities
{
    public class ArticleData
    {
        public int ID { get; set; }
        public int Article_ID { get; set; }
        public string FileName { get; set; }
        public string Filetext { get; set; }
        public string FileHTML { get; set; }
        public int Srn { get; set; }
    }
}