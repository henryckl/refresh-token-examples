using System.Security.Claims;

namespace DotnetApi.services.token
{
  public interface ITokenService
  {
    string GenerateJWTToken(string email);
    string GenerateJWTToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken();
    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    void SaveRefreshToken(string userEmail, string refreshToken);
    void DeleteRefreshToken(string userEmail, string refreshToken);
    string GetRefreshToken(string userEmail);
  }
}