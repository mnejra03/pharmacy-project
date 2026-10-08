using NewPharmacy.Data.Models.Auth;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey(nameof(MyAppUser))]
        public int MyAppUserId { get; set; }
        public MyAppUser? MyAppUser { get; set; }


        public DateTime Date { get; set; }

        public bool Status { get; set; }//aktivna ili završeno
    }
}
