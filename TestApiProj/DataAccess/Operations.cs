
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using TestApiProj.MainEntity;
using TestApiProj.Services;

namespace TestApiProj.DataAccess
{
    public class Operations: IOperations
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        private readonly MyDbContext _context;

        public Operations(HttpClient httpClient, MyDbContext dbContext, IConfiguration configuration)
        {
          _httpClient = httpClient;
            _context = dbContext;
            _configuration = configuration;
        }
        public async Task<List<User>> GetAllAsync()
        {
            string url = "https://jsonplaceholder.typicode.com/users";
            HttpResponseMessage response = await _httpClient.GetAsync(url);         
            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<User>>(responseBody).ToList();
            }
            return new List<User>();
        }

        public async Task<string>  AddUserDetails()
        {
            var list = await GetAllAsync();
            var result = list.Select(user => new User
            {
                email = user.email,
                Password = "sa1234",
                UserRole= "Admin"
            }).ToList();

            foreach (var item in result)
            {
                await _context.Users.AddAsync(item);
            }

            await _context.SaveChangesAsync();
            return "Added Successfully";
        }

        public async Task<string> GenerateAccesToken (User user)
        {
            var claims = new[]
              {
                    new Claim(ClaimTypes.Email, user?.email),
                    new Claim(ClaimTypes.Role, "SuperAdmin") // Add any roles here
                };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: creds
            );

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.WriteToken(token);

            return jwtToken;
        }

        public async Task<string> GenerateRefreshToken()
        {
            var randomNumber = new byte[64];

            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}

