namespace MobileCare.DTOs
{
    public class UpdateRepairOrderDto
    {
        public string? CustomerFullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? IMEI { get; set; }
        public string? ProblemDescription { get; set; }
        public string? DeviceCondition { get; set; }
        public decimal? EstimatedCost { get; set; }
        public string? UserFullName { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? Status { get; set; }
    }
}
