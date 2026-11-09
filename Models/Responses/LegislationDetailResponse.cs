using WebAPI.Models.Entities;

namespace WebAPI.Models.Responses
{
    public class LegislationDetailResponse
    {
        public Legislation Legislation { get; set; }
        public LegislationData Content { get; set; }
    }
}