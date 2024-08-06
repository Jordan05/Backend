namespace MyApi.DTOs
{
    public class AuthResponse
    {
        public required string Token { get; set; }
        public required string Username { get; set; }
    }
}
