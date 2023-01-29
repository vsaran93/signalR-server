using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SignalRChatApp.Models;

namespace SignalRChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignalrDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(SignalrDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public ActionResult Login(LoginUserDto loginUserDto)
        {
            var user = Authenticate(loginUserDto);
            if (user != null)
            {
                var token = GenerateToken(user);
                return Ok(new { token });
            }

            return NotFound("User not found");
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public ActionResult Register(UserDto userDto)
        {
            var registeredUser = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == userDto.UserName.ToLower());

            if (registeredUser == null)
            {

                var newUser = new User
                {
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Email = userDto.Email,
                    Password = userDto.Password,
                    UserName = userDto.UserName,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                _context.Users.Add(newUser);
                _context.SaveChanges();

                var token = GenerateToken(newUser);
                return Ok(new { token });
            }

            return BadRequest("User already exist");
        }

        private string GenerateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.GivenName, user.FirstName)
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private User Authenticate(LoginUserDto loginUserDto)
        {
            var currentUser = _context.Users.FirstOrDefault(x => x.UserName.ToLower() == loginUserDto.UserName.ToLower()
            && x.Password == loginUserDto.Password);

            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }
    }
}
