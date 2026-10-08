using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class WishListDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(WishList))]
        public int WishListId { get; set; }
        public WishList? WishList { get; set; }

        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product? Product { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
