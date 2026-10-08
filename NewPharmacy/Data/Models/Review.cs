using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Data.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; } 

        public string Text { get; set; }

        public int Rating { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }
    }

}
