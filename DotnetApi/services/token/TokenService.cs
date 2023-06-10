using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.services.token
{
  public class TokenService : ITokenService
  {
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    public TokenService()
    {
      _secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? "";
      _issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "";
      _audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "";
    }
    public string GenerateJWTToken(string email)
    {
      var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
      var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
      var claims = new ClaimsIdentity();
      claims.AddClaim(new Claim(JwtRegisteredClaimNames.Email, email));
      var handler = new JwtSecurityTokenHandler();

      var securityToken = handler.CreateToken(new SecurityTokenDescriptor
      {
        Issuer = _issuer,
        Audience = _audience,
        SigningCredentials = signinCredentials,
        Subject = claims,
        NotBefore = DateTime.UtcNow,
        Expires = DateTime.UtcNow.AddMinutes(5),
        IssuedAt = DateTime.UtcNow,
        TokenType = "at+jwt"
      });

      var encodedJwt = handler.WriteToken(securityToken);
      return encodedJwt;
    }
  }
}