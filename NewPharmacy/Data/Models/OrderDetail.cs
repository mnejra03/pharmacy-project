using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class OrderDetail
    {
        [Key]
        public int Id { get; set; }
        public int Qty { get; set; }
        [Precision(18, 2)]
        public decimal PricePerUnit { get; set; }


        [ForeignKey(nameof(Product))]
        public int ProductId { get; set; }
        public Product? Product { get; set; }


        [ForeignKey(nameof(Order))]
        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
