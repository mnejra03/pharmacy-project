namespace NewPharmacy.Data.DTOs
{
    public class ChatGetDTO
    {
        public int Id { get; set; }

        public int SenderId { get; set; }
        public string SenderName { get; set; }

        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }

        public string Message { get; set; }
        public DateTime Date { get; set; }

        public string TypeOfMessage { get; set; }
        public string Status { get; set; }
        public bool IsResponse { get; set; }
    }

}
