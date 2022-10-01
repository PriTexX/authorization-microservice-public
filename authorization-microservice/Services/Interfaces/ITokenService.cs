using System.Security.Claims;
using authorization_microservice.Services.Implementations;

namespace authorization_microservice.Services.Interfaces;

public interface ITokenService
{
    public string PublicKey { get; }
    public string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan lifetime);

    public bool ValidateToken(string token);

    public ClaimsPrincipal ReadToken(string token);

    public string CreateRefreshToken();
}