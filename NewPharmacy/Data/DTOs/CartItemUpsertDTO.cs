namespace NewPharmacy.Data.DTOs
{
    public class CartItemUpsertDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; } = 1;
    }
}
