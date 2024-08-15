using MongoDB.Bson;

public class AuthModel
{
    public ObjectId Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}
