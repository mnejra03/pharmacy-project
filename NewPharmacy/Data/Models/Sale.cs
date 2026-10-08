using Microsoft.EntityFrameworkCore;
using NewPharmacy.Data.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class Sale
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateOfSale { get; set; }


        [ForeignKey(nameof(MyAppUser))]
        public int MyAppUserId { get; set; }
        public MyAppUser? MyAppUser { get; set; }

        [Precision(18, 2)]
        public decimal TotalPrice { get; set; }
    }
}