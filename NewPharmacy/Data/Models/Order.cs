using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using NewPharmacy.Data.Models.Auth;
using Microsoft.EntityFrameworkCore;

namespace NewPharmacy.Data.Models
{

    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public string? PaymentToken { get; set; }
        public string? ShippingAddress { get; set; }

        [ForeignKey(nameof(MyAppUser))]
        public int MyAppUserId { get; set; }
        public MyAppUser? MyAppUser { get; set; }
        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public bool IsSupplyOrder { get; set; } = false;

        public bool IsDeleted { get; set; } = false;

    }

}
