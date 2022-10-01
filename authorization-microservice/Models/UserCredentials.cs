using System.ComponentModel.DataAnnotations;

namespace authorization_microservice.Models;

public class UserCredentials
{
    [Required]
    public string Login { get; set; }
    
    [Required]
    public string Password { get; set; }
}