using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotnetApi.models
{
  public class RefreshTokenInput
  {
    public string? accestoken { get; set; }
    public string? refreshtoken { get; set; }
  }
}