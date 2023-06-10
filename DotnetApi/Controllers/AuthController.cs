using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public IActionResult Login(string email, string password)
    {
      if (email != "email@email.com")
        return Unauthorized();
      var token = _tokenService.GenerateJWTToken(email);
      return Ok(new { token });
    }
  }
}