namespace NewPharmacy.Data.DTOs
{
    public class CreatePaymentIntentDTO
    {
        public int MyAppUserId { get; set; }
        public string? DeliveryMethod { get; set; }
        public List<OrderItemDTO> Items { get; set; } = new();
    }
}
