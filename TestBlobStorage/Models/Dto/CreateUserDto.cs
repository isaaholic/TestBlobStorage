namespace TestBlobStorage.Models.Dto
{
    public class CreateUserDto
    {
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Password { get; set; }
        public int Age { get; set; }
    }
}
