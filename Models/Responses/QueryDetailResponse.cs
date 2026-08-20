using WebAPI.Models.Entities;

namespace WebAPI.Models.Responses
{
    public class QueryDetailResponse
    {
        public Query Query { get; set; }
        public QueryData Content { get; set; }
    }
}