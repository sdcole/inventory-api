using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using InventoryAPI.Models;
using Microsoft.AspNetCore.Http;

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

            // Set the token in a cookie
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                //Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<double>("Jwt:ExpirationMinutes")),
                Expires = DateTime.UtcNow.AddMinutes(15),
                //Secure = true, // Set to true in production to enforce HTTPS
                Secure = false,
                SameSite = SameSiteMode.None // or SameSiteMode.Lax based on your needs
            };

            Response.Cookies.Append("jwt", token, cookieOptions);

            return Ok(new { message = "Login successful" });
        }

        return Unauthorized(new { message = "Invalid credentials" });
    }
}