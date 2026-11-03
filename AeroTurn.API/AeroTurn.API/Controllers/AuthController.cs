using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AeroTurn.API.Models;

namespace AeroTurn.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin userLogin)
        {
            string role = "";
            string airlineContext = "ALL"; // PAA walo ke liye

            if (userLogin.Username == "admin" && userLogin.Password == "paa123")
            {
                role = "TerminalManager";
            }
            else if (userLogin.Username == "gh_user" && userLogin.Password == "gh123")
            {
                role = "GHStaff";
            }
            else if (userLogin.Username == "pia_user" && userLogin.Password == "pia123")
            {
                role = "AirlineOfficer";
                airlineContext = "PIA";
            }
            else if (userLogin.Username == "ek_user" && userLogin.Password == "ek123")
            {
                role = "AirlineOfficer";
                airlineContext = "Emirates";
            }
            else
            {
                return Unauthorized("Invalid Credentials!");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userLogin.Username),
                new Claim(ClaimTypes.Role, role),
                new Claim("AirlineContext", airlineContext)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials);

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                role = role,
                airline = airlineContext,
                username = userLogin.Username
            });
        }
    }
}