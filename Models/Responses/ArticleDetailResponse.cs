using WebAPI.Models.Entities;

namespace WebAPI.Models.Responses
{
    public class ArticleDetailResponse
    {
        public Article Article { get; set; }
        public ArticleData Content { get; set; }
    }
}