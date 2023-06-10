using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotnetApi.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class HelloController : ControllerBase
  {
    private readonly ILogger<HelloController> _logger;

    public HelloController(ILogger<HelloController> logger)
    {
      _logger = logger;
    }
    [HttpGet, Authorize]
    public IActionResult Get()
    {
      var message = "Hello";
      return Ok(new { message });
    }
  }
}