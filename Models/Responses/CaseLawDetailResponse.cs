using WebAPI.Models.Entities;

namespace WebAPI.Models.Responses
{
    public class CaseLawDetailResponse
    {
        public CaseLaw CaseLaw { get; set; }
        public CaseLawData Content { get; set; }
    }
}