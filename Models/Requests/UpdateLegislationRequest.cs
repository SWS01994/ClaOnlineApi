namespace WebAPI.Models.Requests
{
    public class UpdateLegislationRequest : SaveLegislationRequest
    {
        public int id { get; set; }
    }
}