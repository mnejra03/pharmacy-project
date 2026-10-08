using System.ComponentModel.DataAnnotations;
using NewPharmacy.Data.Validation;

namespace NewPharmacy.Data.DTOs
{
    public class ProductAddDTO
    {
        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        public string Name { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [PositiveDecimal(0.01, ErrorMessage = "Price must be greater than 0.")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative.")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Picture URL is required.")]
        public string Picture { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

        public bool IsDiscounted { get; set; }

        [Range(1, 100, ErrorMessage = "Discount must be between 1 and 100.")]
        public decimal? DiscountPercentage { get; set; }

        public int? BrandId { get; set; }
    }
}
