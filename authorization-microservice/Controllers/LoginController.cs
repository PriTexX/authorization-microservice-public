using System.Security.Claims;
using authorization_microservice.DatabaseModels;
using authorization_microservice.Models;
using authorization_microservice.Repositories.Interfaces;
using authorization_microservice.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace authorization_microservice.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : Controller
{
    private readonly IActiveDirectoryService _activeDirService;
    private readonly ITokenService _tokenService;
    private readonly IBaseRepository<User> _userRepository;

    public LoginController(
        IActiveDirectoryService activeDirService, 
        ITokenService tokenService, 
        IBaseRepository<User> userRepository
    )
    {
        _userRepository = userRepository;
        _tokenService = tokenService;
        _activeDirService = activeDirService;
    }

    [HttpPost(Name = "login")]
    public IActionResult Auth(UserCredentials credentials)
    {
        var user = _activeDirService.AuthenticateUser(credentials.Login, credentials.Password);

        if (user is null)
        {
            return BadRequest("Wrong username or password");
        }

        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Role, user.Value.Role),
            new Claim("userGuid", user.Value.Guid)
        };

        var accessToken = _tokenService.CreateAccessToken(claims, TimeSpan.FromMinutes(10));
        var refreshToken = _tokenService.CreateRefreshToken();

        var repoUser = _userRepository.Get(user.Value.Guid);

        if (repoUser is null)
        {
            _userRepository.Create(new User
            {
                Guid = user.Value.Guid,
                Role = user.Value.Role,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(10),
            });
        }
        else
        {
            repoUser.Guid = user.Value.Guid;
            repoUser.Role = user.Value.Role;
            repoUser.RefreshToken = refreshToken;
            repoUser.RefreshTokenExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(30);

            _userRepository.Update(repoUser);
        }

        return new ObjectResult(
            new AuthenticatedResponse { RefreshToken = refreshToken, AccessToken = accessToken });
    }
}