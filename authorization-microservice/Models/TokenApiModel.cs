using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace authorization_microservice.Models;

public class TokenApiModel
{
    [Required]
    [NotNull]
    public string? AccessToken { get; set; }
    
    [Required]
    [NotNull]
    public string? RefreshToken { get; set; }
}