using NewPharmacy.Data.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Time { get; set; } = DateTime.UtcNow;
        public bool Read { get; set; } = false;

        
        [ForeignKey(nameof(MyAppUser))]
        public int MyAppUserId { get; set; }
        public MyAppUser? MyAppUser { get; set; }

        [ForeignKey(nameof(Order))]
        public int? OrderId { get; set; } 
        public Order? Order { get; set; }

        
        public string Type { get; set; } = string.Empty;
        public int? SenderId { get; set; }

    }
}
