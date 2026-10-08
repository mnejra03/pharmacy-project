namespace NewPharmacy.Data.DTOs
{
    public class FullNotificationDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public bool Read { get; set; }
        public int MyAppUserId { get; set; }
        public int OrderId { get; set; }
        public string Type { get; set; } = string.Empty;
        public NotificationOrderDTO? Order { get; set; }
        public List<NotificationOrderDetailDTO>? OrderDetails { get; set; }
        public int? SenderId { get; set; }
    }

    public class NotificationOrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string? ShippingAddress { get; set; }
        public int MyAppUserId { get; set; }
        public bool IsSupplyOrder { get; set; }
    }

    public class NotificationOrderDetailDTO
    {
        public int Id { get; set; }
        public int Qty { get; set; }
        public decimal PricePerUnit { get; set; }
        public NotificationProductDTO? Product { get; set; }
    }

    public class NotificationProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
