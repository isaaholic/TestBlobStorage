using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using TestBlobStorage.Data;
using TestBlobStorage.Models;
using TestBlobStorage.Models.Dto;

namespace TestBlobStorage.Services
{
    public class UserService : IUserService
    {
        private readonly ServerDbContext _context;
        private readonly IStorageManager _storageManager;

        public UserService(ServerDbContext context, IStorageManager storageManager)
        {
            _context = context;
            _storageManager = storageManager;
        }
        public async Task<User> GetUser(Guid userId)
            => await _context.Users.FirstAsync(u => u.Id == userId);

        public async Task<string> Login(AuthUserDto request)
        {
            var employee = await _context.Users.FirstOrDefaultAsync(e => e.Nickname == request.Nickname);

            if (employee is null) throw new ($"{request.Nickname} does not exist");

            if (!VerifyPassword(request.Password, employee.PasswordHash, employee.PasswordSalt)) throw new ("Password doesn't match");

            var token = CreateToken(employee);

            return token;
        }

        public async Task<bool> Register(CreateUserDto request)
        {
            if ((await _context.Users.FirstOrDefaultAsync(u => u.Nickname == request.Nickname)) is not null)
                throw new ("Username already taken");
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

            return true;
        }

        public async Task<bool> UpdateUser(UpdateUserDto request)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.Id);

            if (existUser is null) throw new ("user doesn't exist");

            existUser.Name = request.Name;
            existUser.Surname = request.Surname;
            existUser.Age = request.Age;
            existUser.Nickname = request.Nickname;

            _context.Update(existUser);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<string> UploadProfilePicture(UpdateProfilePictureDto request)
        {
            var existUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId);

            if (existUser is null) throw new("user doesn't exist");

            var file = request.Image;

            using (var stream = file.OpenReadStream())
            {
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var contentType = file.ContentType;

                var result = _storageManager.UploadFile(stream, fileName, contentType);

                var fileUrl = await _storageManager.GetSadeUrlAsync(fileName);
                var fileSignedUrl = await _storageManager.GetSignedUrlAsync(fileName);

                if (string.IsNullOrWhiteSpace(fileUrl))
                    throw new ("Something went wrong while uploading photo");

                existUser.ProfilePhotoUrl = fileUrl;
                _context.Update(existUser);
                await _context.SaveChangesAsync();

                if (result)
                {
                    return fileSignedUrl;
                }
                else
                {
                    throw new ("File upload failed.");
                }
            }
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
