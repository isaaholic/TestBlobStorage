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
using TestBlobStorage.Services;

namespace TestBlobStorage.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ServerDbContext _context;
        private readonly IStorageManager _storageManager;

        // isaaholic
        // pass:Mahal2003
        // id = 9cb1b314-cde0-4a15-4cdd-08dbc8edbedd

        public UsersController(ServerDbContext context, IStorageManager storageManager)
        {
            _context = context;
            _storageManager = storageManager;
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

            if (!VerifyPassword(request.Password, employee.PasswordHash, employee.PasswordSalt)) return BadRequest("Password doesn't match");

            var token = CreateToken(employee);

            return Ok(token);
        }

        [HttpGet("getUser")]
        public async Task<ActionResult> GetUser(Guid userId)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(u=>u.Id == userId);
            if (existUser is null) return NotFound("User doesnt exists");

            return Ok(existUser);
        }

        [HttpPut("uploadPhoto")]
        public async Task<ActionResult> UploadImage([FromForm] UpdateProfilePictureDto request)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (existUser is null) return NotFound("user doesn't exist");

            var file = request.Image;

            using (var stream = file.OpenReadStream())
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var contentType = file.ContentType;

                var result = _storageManager.UploadFile(stream, fileName, contentType);

                var fileUrl = _storageManager.GetSignedUrl(fileName);

                if (string.IsNullOrWhiteSpace(fileUrl))
                    return BadRequest("Something went wrong while uploading photo");

                existUser.ProfilePhotoUrl = fileUrl;
                _context.Update(existUser);
                await _context.SaveChangesAsync();

                if (result)
                {
                    return Ok($"File uploaded successfully. Link: {fileUrl}");
                }
                else
                {
                    return BadRequest("File upload failed.");
                }
            }

        }

        [HttpPut("updateUser")]
        public async Task<ActionResult> UpdateUser(UpdateUserDto request)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

            if (existUser is null) return NotFound("user doesn't exist");

            existUser.Name = request.Name;
            existUser.Surname = request.Surname;
            existUser.Age = request.Age;
            existUser.Nickname = request.Nickname;

            _context.Update(existUser);
            await _context.SaveChangesAsync();

            return Ok(true);
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
