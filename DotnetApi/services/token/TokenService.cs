using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DotnetApi.models;
using Microsoft.IdentityModel.Tokens;

namespace DotnetApi.services.token
{
  public class TokenService : ITokenService
  {
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private TimeSpan expireAccessToken = TimeSpan.FromMinutes(2);
    private TimeSpan expireRefreshToken = TimeSpan.FromMinutes(2);
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
        Expires = DateTime.UtcNow.Add(expireAccessToken),
        IssuedAt = DateTime.UtcNow,
        TokenType = "at+jwt"
      });

      var encodedJwt = handler.WriteToken(securityToken);
      return encodedJwt;
    }

    public string GenerateJWTToken(IEnumerable<Claim> claims)
    {
      var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
      var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
      var handler = new JwtSecurityTokenHandler();

      var securityToken = handler.CreateToken(new SecurityTokenDescriptor
      {
        Issuer = _issuer,
        Audience = _audience,
        SigningCredentials = signinCredentials,
        Subject = new ClaimsIdentity(claims),
        NotBefore = DateTime.UtcNow,
        Expires = DateTime.UtcNow.Add(expireAccessToken),
        IssuedAt = DateTime.UtcNow,
        TokenType = "at+jwt"
      });

      var encodedJwt = handler.WriteToken(securityToken);
      return encodedJwt;
    }

    public string GenerateRefreshToken()
    {
      var randomNumber = new byte[32];
      using var rng = RandomNumberGenerator.Create();
      rng.GetBytes(randomNumber);
      return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateAudience = false, //you might want to validate the audience and issuer depending on your use case
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("mysec3tKey@457Exemple")),
        ValidateLifetime = false //here we are saying that we don't care about the token's expiration date
      };
      var tokenHandler = new JwtSecurityTokenHandler();
      SecurityToken securityToken;
      var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
      var jwtSecurityToken = securityToken as JwtSecurityToken;
      if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        throw new SecurityTokenException("Invalid token");
      return principal;
    }

    public void SaveRefreshToken(string userEmail, string refreshToken)
    {
      TokenService.SaveRefreshTokenInMemory(userEmail, refreshToken);
    }

    public void DeleteRefreshToken(string userEmail, string refreshToken)
    {
      TokenService.DeleteRefreshTokenInMemory(userEmail, refreshToken);
    }

    public string GetRefreshToken(string userEmail)
    {
      return GetRefreshTokenInMemory(userEmail);
    }

    // Handle refresh token
    private static List<RefreshTokenModel> _refreshTokens = new();

    private static void SaveRefreshTokenInMemory(string userEmail, string refreshToken)
    {
      var refreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(10);
      var refresh = new RefreshTokenModel()
      { Email = userEmail, RefreshToken = refreshToken, RefreshTokenExpiryTime = refreshTokenExpiryTime };
      _refreshTokens.Add(refresh);
    }

    public static string GetRefreshTokenInMemory(string userEmail)
    {
      var lastToken = _refreshTokens.LastOrDefault(x => x.Email == userEmail && x.RefreshTokenExpiryTime >= DateTime.UtcNow);
      if (lastToken != null && lastToken.RefreshToken != null)
      {
        return lastToken.RefreshToken;
      }
      return "";
    }

    public static void DeleteRefreshTokenInMemory(string userEmail, string refreshToken)
    {
      var item = _refreshTokens.FirstOrDefault(x => x.Email == userEmail && x.RefreshToken == refreshToken);
      if (item != null)
        _refreshTokens.Remove(item);
    }
  }
}