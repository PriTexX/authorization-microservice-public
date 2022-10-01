using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using authorization_microservice.Services.Interfaces;

namespace authorization_microservice.Services.Implementations;

public class TokenService : ITokenService
{
    public string PublicKey { get; private set; }
    private readonly TokenServiceConfiguration _settings;
    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new();

    public TokenService(TokenServiceConfiguration settings)
    {
        _settings = settings;
        PublicKey = settings.RsaPublicKey;
    }

    public string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan lifetime)
    {
        var privateKey = _settings.RsaPrivateKey.ToCharArray();

        using var rsa = RSA.Create();
        rsa.ImportFromPem(privateKey);

        var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        {
            CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
        };

        var now = DateTime.UtcNow;

        var jwt = new JwtSecurityToken(
            claims: claims,
            expires: now + lifetime,
            signingCredentials: signingCredentials,
            issuer: "testIss"
        );

        return _jwtSecurityTokenHandler.WriteToken(jwt);
    }

    public bool ValidateToken(string token)
    {

        var publicKey = _settings.RsaPublicKey.ToCharArray();

        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            }
        };

        try
        {
            var handler = new JwtSecurityTokenHandler();
            handler.ValidateToken(token, validationParameters, out var validatedSecurityToken);
        }
        catch
        {
            return false;
        }

        return true;
    }

    public ClaimsPrincipal ReadToken(string token)
    { 
        var publicKey = _settings.RsaPublicKey.ToCharArray();

        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false, 
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new RsaSecurityKey(rsa),
            CryptoProviderFactory = new CryptoProviderFactory
            {
                CacheSignatureProviders = false
            },
            ValidateLifetime = false 
        };
        
        var principal = _jwtSecurityTokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(
                SecurityAlgorithms.RsaSha256,
                    StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");
        
        return principal;
    }

    public string CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}
