namespace MyApi.DTOs
{
    public class UserDTO
    {
        public required int Id { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
