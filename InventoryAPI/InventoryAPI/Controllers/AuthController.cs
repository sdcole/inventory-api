using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using InventoryAPI.Models;
using System;

namespace InventoryAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly JwtHelper _jwtHelper;

        public AuthController(IConfiguration configuration, JwtHelper jwtHelper)
        {
            _configuration = configuration;
            _jwtHelper = jwtHelper;
        }

        [HttpPost("Login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            // Validate user credentials
            if (loginRequest.username == "test" && loginRequest.password == "test") // Replace with your validation logic
            {
                var token = _jwtHelper.GenerateToken(loginRequest.username);
                return Ok(new { token });
            }

            return Unauthorized(new { message = "Invalid credentials" });
        }
    }
}