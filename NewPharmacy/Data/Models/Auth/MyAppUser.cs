using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace NewPharmacy.Data.Models.Auth;

public class MyAppUser
{
    [Key]
    public int ID { get; set; }
    public string Username { get; set; }
    [JsonIgnore]
    public string Password { get; set; }

    
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string? PhoneNumber { get; set; }

    public bool IsAdmin { get; set; }
    public bool IsPharmacist { get; set; }
    public bool IsCustomer { get; set; }

    
    public DateTime? EmploymentDate { get; set; }  
    public string? Email { get; set; }
    public string? ProfileImageUrl { get; set; }

    public bool IsDeleted { get; set; } = false;

}
