namespace MobileCare.DTOs
{
    public class UpdateRepairOrderDto
    {
        public string? Status { get; set; }
        public string? Notes { get; set; }
        public decimal? EstimatedCost { get; set; }
    }
}
