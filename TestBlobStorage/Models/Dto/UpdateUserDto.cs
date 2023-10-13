namespace TestBlobStorage.Models.Dto
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
    }
}
