using WebAPI.Models.Entities;

namespace WebAPI.Models.Responses
{
    public class CircularDetailResponse
    {
        public Circular Circular { get; set; }
        public CircularData Content { get; set; }
    }
}