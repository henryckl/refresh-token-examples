using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DotnetApi.models;
using DotnetApi.services.token;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class AuthController : ControllerBase
  {
    private readonly ITokenService _tokenService;

    public AuthController(ITokenService tokenService)
    {
      _tokenService = tokenService;
    }

    [HttpPost("login")]
    public IActionResult Login(LoginInput user)
    {
      if (user.Email != "email@email.com")
        return Unauthorized();
      var accestoken = _tokenService.GenerateJWTToken(user.Email);
      var refreshtoken = _tokenService.GenerateRefreshToken();
      _tokenService.SaveRefreshToken(user.Email, refreshtoken);
      return Ok(new { accestoken, refreshtoken });
    }
    [HttpPost("refresh")]
    public IActionResult Refresh(RefreshTokenInput token)
    {
      if (string.IsNullOrEmpty(token.accestoken) || string.IsNullOrEmpty(token.refreshtoken))
        return BadRequest();
      var principal = _tokenService.GetPrincipalFromExpiredToken(token.accestoken);
      var email = principal.FindFirstValue(ClaimTypes.Email);
      var savedRefreshToken = _tokenService.GetRefreshToken(email ?? "");
      if (savedRefreshToken != token.refreshtoken)
        return BadRequest();

      var newJwtToken = _tokenService.GenerateJWTToken(principal.Claims);
      var newRefreshToken = _tokenService.GenerateRefreshToken();
      _tokenService.DeleteRefreshToken(email ?? "", token.refreshtoken);
      _tokenService.SaveRefreshToken(email ?? "", newRefreshToken);

      return new ObjectResult(new
      {
        token = newJwtToken,
        refreshToken = newRefreshToken
      });
    }
  }
}