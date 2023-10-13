using TestBlobStorage.Models;
using TestBlobStorage.Models.Dto;

namespace TestBlobStorage.Services
{
    public interface IUserService
    {
        Task<User> GetUser(Guid userId);
        Task<string> UploadProfilePicture(UpdateProfilePictureDto request);
        Task<bool> UpdateUser(UpdateUserDto request);
        Task<bool> Register(CreateUserDto request);
        Task<string> Login(AuthUserDto request);
    }
}
