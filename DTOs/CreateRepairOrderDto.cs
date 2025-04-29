namespace MobileCare.DTOs
{
    public class CreateRepairOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string DeviceBrand { get; set; } = string.Empty;
        public string DeviceModel { get; set; } = string.Empty;
        public string IMEI { get; set; } = string.Empty ;
        public string ProblemDescription { get; set; } = string.Empty;
        public string DeviceCondition { get; set; } = string.Empty;
        public decimal EstimatedCost { get; set; }

        public int UserId { get; set; }
        public string Notes { get; set; }

    }
}
