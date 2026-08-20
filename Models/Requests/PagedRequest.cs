namespace WebAPI.Models.Requests
{
    public class PagedRequest
    {
        public int PageNumber { get; set; }   // 1-based
        public int PageSize { get; set; }

        // Sensible defaults applied in one place, so no service
        // has to repeat the "if < 1" guard for paging.
        public void Normalize()
        {
            if (this.PageNumber < 1) this.PageNumber = 1;
            if (this.PageSize < 1) this.PageSize = 20;
            if (this.PageSize > 200) this.PageSize = 200;   // safety cap
        }
    }
}