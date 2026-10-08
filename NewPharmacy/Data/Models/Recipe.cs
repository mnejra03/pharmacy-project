using NewPharmacy.Data.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class Recipe
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateOfIssue { get; set; }
        public string DoctorFirstname { get; set; }
        public string DoctorLastname { get; set; }


        [ForeignKey(nameof(MyAppUser))]
        public int MyAppUserId { get; set; }
        public MyAppUser? MyAppUser { get; set; }

        public byte[]? Scan { get; set; } 
        public string Status { get; set; } = "Pending";

    }
}