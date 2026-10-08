using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Data.Models.Auth
{
    public class UpdateMyAppUserDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        public int ID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        public bool IsAdmin { get; set; }
        public bool IsPharmacist { get; set; }
        public bool IsCustomer { get; set; }
    }
}
