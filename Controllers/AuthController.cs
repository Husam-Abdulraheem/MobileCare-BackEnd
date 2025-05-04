using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileCare.DTOs;
using MobileCare.Models;

namespace MobileCare.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly AppDbContext _db;

        public AuthController(JwtService jwtService, AppDbContext db)
        {
            _jwtService = jwtService;
            _db = db;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDTO login)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == login.Email && u.PasswordHash == login.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            var token = _jwtService.GenerateToken(user);
            return Ok(new { token });
        }
    }
}
