namespace TestBlobStorage.Models.Dto
{
    public class UpdateProfilePictureDto
    {
        public Guid Id { get; set; }
        public IFormFile Image { get; set; }
    }
}
