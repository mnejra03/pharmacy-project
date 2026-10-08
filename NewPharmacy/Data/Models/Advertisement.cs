using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Data.Models
{
    public class Advertisement
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string imageURL { get; set; }
    }
}
