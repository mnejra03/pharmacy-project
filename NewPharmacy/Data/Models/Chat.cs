using NewPharmacy.Data.Models.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models
{
    public class Chat
    {
        public int Id { get; set; }


        [ForeignKey(nameof(MyAppUser))]
        public int SenderId { get; set; }
        public MyAppUser? Sender { get; set; }


        [ForeignKey(nameof(Receiver))]
        public int? ReceiverId { get; set; }
        public MyAppUser? Receiver { get; set; }


        public string Message { get; set; }
        public DateTime Date { get; set; }
        public string TypeOfMessage { get; set; }//pitanje,odgovor
        public string Status { get; set; }
        public bool IsResponse { get; set; } = false; // true ako je odgovor farmaceuta
    }
}
