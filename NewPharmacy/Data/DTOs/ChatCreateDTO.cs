using System.ComponentModel.DataAnnotations;

namespace NewPharmacy.Data.DTOs
{
    public class ChatCreateDTO
    {
        [Range(1, int.MaxValue, ErrorMessage = "ReceiverId must be greater than 0.")]
        public int ReceiverId { get; set; }

        [Required]
        [StringLength(1000, MinimumLength = 1)]
        public string Message { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string TypeOfMessage { get; set; } = "question";

        [Required]
        [StringLength(50, MinimumLength = 1)]
        public string Status { get; set; } = "neprocitano";

        public bool IsResponse { get; set; } = false;
    }
}
