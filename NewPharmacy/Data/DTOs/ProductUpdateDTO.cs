using System.ComponentModel.DataAnnotations;
using NewPharmacy.Data.Validation;

namespace NewPharmacy.Data.DTOs
{
    public class ProductUpdateDTO
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [PositiveDecimal(0.01, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Picture URL is required.")]
        public string Picture { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        public int? BrandId { get; set; }

        public bool IsDiscounted { get; set; }

        [Range(1, 100, ErrorMessage = "Discount must be between 1 and 100.")]
        public decimal? DiscountPercentage { get; set; }
    }

}
