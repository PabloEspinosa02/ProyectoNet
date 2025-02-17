using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TiendaUT.Context;
using TiendaUT.Domain;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TiendaUT.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            if (await UserExists(userDto.Username))
            {
                return BadRequest("Username is taken");
            }

            User user = new User()
            {
                Username = userDto.Username.ToLower(),
                PasswordHash = userDto.Password,  // Almacenar contrase�a en texto plano
                Email = userDto.Email,
                IdRole = userDto.idRole // Usar el rol especificado o "user" por defecto
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { username = user.Username, role = user.Role });
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(LoginDto loginDto)
        {
            var user = await _context.Users.Include(x => x.Role).SingleOrDefaultAsync(x => x.Username == loginDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username");
            }

            if (loginDto.Password != user.PasswordHash)
            {
                return Unauthorized("Invalid password");
            }

            var token = CreateToken(user);
            return Ok(new { token, usuario = user, role = user.Role.NameRole });
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.Username == username.ToLower());
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Username),
                new Claim(ClaimTypes.Role, user.Role.NameRole)  // Agregar el rol al token
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }

    public class UserDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int idRole { get; set; } // Agrega esta l�nea
    }

    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    
}
