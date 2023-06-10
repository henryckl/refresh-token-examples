using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetApi.services.token
{
  public interface ITokenService
  {
    string GenerateJWTToken(string email);
  }
}