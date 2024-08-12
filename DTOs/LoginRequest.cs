namespace MyApi.DTOs
{
    public class AuthRequest
    {
        public required int Id { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
    }
}
