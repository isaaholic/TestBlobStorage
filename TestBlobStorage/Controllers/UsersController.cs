using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TestBlobStorage.Data;
using TestBlobStorage.Models;
using TestBlobStorage.Models.Dto;

namespace TestBlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ServerDbContext _context;

        public UsersController(ServerDbContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register(CreateUserDto request)
        {
            if (_context.Users.Any(u => u.Nickname == request.Nickname))
                return BadRequest("Username already taken");
            User user = new();
            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordSalt = passwordSalt;
            user.PasswordHash = passwordHash;
            user.Name = request.Name;
            user.Surname = request.Surname;
            user.Nickname = request.Nickname;
            user.Age = request.Age;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(AuthUserDto request)
        {
            var employee = await _context.Users.FirstOrDefaultAsync(e => e.Nickname == request.Nickname);

            if (employee is null) return NotFound($"{request.Nickname} does not exist");

            if (!VerifyPassword(request.Password, employee.PasswordHash, employee.PasswordSalt)) return BadRequest("Password doesn't match Admin");

            var token = CreateToken(employee);

            return Ok(token);
        }

        [HttpPost("uploadPhoto")]
        public async Task<ActionResult> UploadImage(UpdateProfilePictureDto request)
        {
            
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computeHash.SequenceEqual(passwordHash);
            }
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Role,nameof(User)),
                new Claim(ClaimTypes.Name,user.Nickname)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("SuperSecretdiamamuellimgorsuneziyyetcekmesindeyeburayazdimyenimuellimsizidusunuremmmmmmmm"));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                signingCredentials: creds,
                expires: DateTime.Now.AddHours(1)
             );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
    }
}
