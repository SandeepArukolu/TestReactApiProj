using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using TestApiProj.DTOS;
using TestApiProj.MainEntity;
using TestApiProj.Models;
using TestApiProj.Services;

namespace TestApiProj.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IOperations _operations;
        private readonly IConfiguration _configuration;
        private readonly MyDbContext _context;
        public TestController(IOperations operations, IConfiguration configuration, MyDbContext context)
        {
            _operations = operations;
            _configuration = configuration;
            _context = context;
        }

        [HttpGet("GetAllusers")]
        public async Task<ActionResult> GetAllusers()
        {
            var result = await _operations.GetAllAsync();
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {

            var LoginUser = _context.Users.FirstOrDefault(x => x.email.Equals(userLogin.Username));

            if (LoginUser is not null)
            {
                var claims = new[]
               {
                    new Claim(ClaimTypes.Email, LoginUser.email),
                    new Claim(ClaimTypes.Role, "Admin") // Add any roles here
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(20),
                    signingCredentials: creds
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.WriteToken(token);

                return Ok(new AuthResponse { Token = jwtToken });
            }

            return Unauthorized("Invalid username or password");
        }
        [HttpPost("AddUsers")]
        public async Task<IActionResult> AddUsers()
        {
           var val = await _operations.AddUserDetails();
            return Ok(val);
                
        }



        
    }
}



