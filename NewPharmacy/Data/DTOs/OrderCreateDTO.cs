using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Data.DTOs
{
    public class OrderItemDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "ProductId must be greater than 0.")]
        public int ProductId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Qty must be greater than 0.")]
        public int Qty { get; set; }
    }

    public class OrderCreateDTO
    {
        [Required]
        [StringLength(200, MinimumLength = 2)]
        public string Address { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string PostalCode { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Country { get; set; } = string.Empty;

        [StringLength(50)]
        public string? PaymentMethod { get; set; }

        [StringLength(500)]
        public string? PaymentToken { get; set; }

        [StringLength(50)]
        public string? DeliveryMethod { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one order item is required.")]
        public List<OrderItemDTO> Items { get; set; } = new();
    }
}
