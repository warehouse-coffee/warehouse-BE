namespace warehouse_BE.Application.Common.Models
{
    public class Page
    {
        public int Size { get; set; }
        public int PageNumber { get; set; } 
        public int TotalElements { get; set; }
        public int TotalPages => Size > 0 ? (int)Math.Ceiling((double)TotalElements / Size) : 0;
        public string SortBy { get; set; } = "Id"; 
        public bool SortAsc { get; set; } = true; 
    }
}
