namespace NewPharmacy.Data.Models.Auth
{
    public class CreateMyAppUserDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsPharmacist { get; set; }
        public bool IsCustomer { get; set; }
    }
}
