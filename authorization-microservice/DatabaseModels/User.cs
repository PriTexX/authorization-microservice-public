using System.ComponentModel.DataAnnotations;

namespace authorization_microservice.DatabaseModels;

public class User : ABCBaseModel
{
    [Key]
    [Required]
    public override string Guid { get; set; }

    [Required]
    public string RefreshToken { get; set; }
    
    [Required]
    public DateTime RefreshTokenExpiryTime { get; set; }
    
    [Required]
    public string Role { get; set; }
}