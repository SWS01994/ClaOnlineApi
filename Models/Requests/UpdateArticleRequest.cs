namespace WebAPI.Models.Requests
{
    public class UpdateArticleRequest : SaveArticleRequest
    {
        public int ID { get; set; }
    }
}