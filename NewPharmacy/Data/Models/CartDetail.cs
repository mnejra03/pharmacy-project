using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class CartDetail
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey(nameof(Cart))]
        public int CartId { get; set; }
        public Cart? Cart { get; set; }


        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product? Product { get; set; }


        public int Quantity { get; set; }
        [Precision(18, 2)]
        public decimal Price { get; set; }
    }
}
