namespace MobileCare.DTOs
{
    public class RepairOrderTrackingDto
    {
        public int RepairOrderId { get; set; }
        public string CustomerName { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string IMEI { get; set; }
        public string ProblemDescription { get; set; }
        public string Status { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
