using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NewPharmacy.Data.Models.Auth;

public class MyAuthenticationToken
{
    [Key]
    public int ID { get; set; }

    public required string Value { get; set; } 

    public string IpAddress { get; set; } = string.Empty;

    public DateTime RecordedAt { get; set; } 

    
    [ForeignKey(nameof(MyAppUser))]
    public int MyAppUserId { get; set; }

    public MyAppUser? MyAppUser { get; set; } 
}
