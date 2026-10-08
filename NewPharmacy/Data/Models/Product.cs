using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
        public int QuantityInStock { get; set; }
        public string Picture { get; set; }


        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }

        public bool IsDiscounted { get; set; } = false;
        public decimal? DiscountPercentage { get; set; }
        [Precision(18, 2)]
        public decimal DiscountedPrice { get; set; }

        public DateTime? DatumDodavanja { get; set; }

        public void UpdateDiscountedPrice()
        {
            DiscountedPrice = IsDiscounted && DiscountPercentage.HasValue
                ? decimal.Round(Price * (1 - (DiscountPercentage.Value / 100m)), 2, MidpointRounding.AwayFromZero)
                : Price;
        }

        [ForeignKey(nameof(Brand))]
        public int? BrandId { get; set; }
        public Brand? Brand { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public List<Review> Reviews { get; set; }
    }
}
