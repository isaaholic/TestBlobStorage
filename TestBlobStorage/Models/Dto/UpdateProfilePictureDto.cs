namespace TestBlobStorage.Models.Dto
{
    public class UpdateProfilePictureDto
    {
        public Guid UserId { get; set; }
        public IFormFile Image { get; set; }
    }
}
