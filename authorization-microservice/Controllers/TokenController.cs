using authorization_microservice.DatabaseModels;
using authorization_microservice.Models;
using authorization_microservice.Repositories.Interfaces;
using authorization_microservice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace authorization_microservice.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TokenController : Controller
{
    private readonly ITokenService _tokenService;
    private readonly IBaseRepository<User> _userRepository;

    public TokenController(ITokenService tokenService, IBaseRepository<User> userRepository)
    {
        _tokenService = tokenService;
        _userRepository = userRepository;
    }
    
    [HttpPost]
    public IActionResult RefreshToken(TokenApiModel tokenApiModel)
    {
        var accessToken = tokenApiModel.AccessToken;
        var refreshToken = tokenApiModel.RefreshToken;
        
        var principal = _tokenService.ReadToken(accessToken);
        var userGuid = principal.Claims.First(claim => claim.Type == "userGuid").Value;

        var user = _userRepository.Get(userGuid);
        if (user is null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return BadRequest("Invalid client request");
        
        var newAccessToken = _tokenService.CreateAccessToken(principal.Claims, TimeSpan.FromMinutes(10));
        var newRefreshToken = _tokenService.CreateRefreshToken();
        
        user.RefreshToken = newRefreshToken;
        _userRepository.Update(user);
        
        return Ok(new AuthenticatedResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }

    [HttpGet]
    public string PublicKey()
    {
        return _tokenService.PublicKey;
    }
}