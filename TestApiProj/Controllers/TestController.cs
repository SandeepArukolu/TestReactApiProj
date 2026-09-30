using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
        private readonly MyDbContext _context;
        private readonly IMapper _mapper;
        public TestController(IOperations operations, MyDbContext context, IMapper mapper)
        {
            _operations = operations;
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("GetAllusers")]
        public async Task<ActionResult> GetAllusers()
        {
            var result = await _operations.GetAllAsync();
            return Ok(result);
        }

        //[HttpPost("login")]
        //public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        //{
        //    try
        //    {
        //        var Users = await _operations.GetAllAsync();
        //        userLogin.Username = "Sincere@april.biz";
        //        var LoginUser = Users.FirstOrDefault(x => x.email.Equals(userLogin.Username));

        //        if (LoginUser is not null)
        //        {
        //            string accesToken = await _operations.GenerateAccesToken(LoginUser);
        //            string refreshToken = await _operations.GenerateRefreshToken();

        //            var refreshTokenObject = new RefreshTokensDTO
        //            {
        //                Token = refreshToken,
        //                UserId = LoginUser.Id.ToString(),
        //                Expires = DateTime.UtcNow.AddDays(7)
        //            };

        //            var checkRefreshToken = Request.Cookies["refreshToken"];

        //            var mapping = _mapper.Map<RefreshToken>(refreshTokenObject);

        //            _context.RefreshTokens.Add(mapping);
        //            _context.SaveChanges();

        //            Response.Cookies.Append(
        //           "refreshToken",
        //           refreshToken,
        //           new CookieOptions
        //           {
        //               HttpOnly = true,
        //               Secure = true,
        //               SameSite = SameSiteMode.None,
        //               Expires = DateTime.UtcNow.AddDays(7),
        //               Path = "/"
        //           });

        //            return Ok(new AuthResponse { Token = accesToken });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }

        //    return Unauthorized("Invalid username or password");
        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto userLogin)
        {
            try
            {
                var users = await _operations.GetAllAsync();

                userLogin.Username = "Sincere@april.biz";

                var loginUser = users.FirstOrDefault(x =>
                    x.email.Equals(
                        userLogin.Username,
                        StringComparison.OrdinalIgnoreCase
                    )
                );

                if (loginUser is null)
                {
                    return Unauthorized("Invalid username or password");
                }

                // Generate access token
                string accessToken =
                    await _operations.GenerateAccesToken(loginUser);

                // Generate refresh token
                string refreshToken =
                    await _operations.GenerateRefreshToken();

                var refreshTokenObject = new RefreshTokensDTO
                {
                    Token = refreshToken,
                    UserId = loginUser.Id.ToString(),
                    Expires = DateTime.UtcNow.AddDays(7)
                };

                var refreshTokenEntity =
                    _mapper.Map<RefreshToken>(refreshTokenObject);

                // Save refresh token in database
                //await _context.RefreshTokens.AddAsync(refreshTokenEntity);
                //await _context.SaveChangesAsync();

                //// Store refresh token in HttpOnly cookie
                //Response.Cookies.Append(
                //    "refreshToken",
                //    refreshToken,
                //    new CookieOptions
                //    {
                //        HttpOnly = true,
                //        Secure = true,
                //        SameSite = SameSiteMode.None,
                //        Expires = DateTimeOffset.UtcNow.AddDays(7),
                //        Path = "/",
                //        IsEssential = true
                //    }
                //);


                return Ok(new AuthResponse
                {
                    Token = accessToken
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddUsers")]
        public async Task<IActionResult> AddUsers()
        {
            var val = await _operations.AddUserDetails();
            return Ok(val);

        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> RefreshToken()
        {
                var refreshToken =
                Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized();

            var storedToken =  await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshToken);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == Convert.ToInt32(storedToken.UserId));

            if (storedToken == null)
            {
                return Unauthorized();
            }

            var newAccessToken = user != null ? 
                _operations.GenerateAccesToken(
                    user) : null;

            return Ok(new
            {
                AccessToken = newAccessToken
            });
        }

    }
}



