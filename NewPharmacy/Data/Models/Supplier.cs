using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Data.Models
{
    public class Supplier
    {
        [Key]
        public int Id  { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
