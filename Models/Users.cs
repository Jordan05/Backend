namespace MyApi.Models
{
    public class User
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
}
