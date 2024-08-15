using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MyApi.Models
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        
        public required string Email { get; set; }
        
        public required string Password { get; set; }
        
        public required string Name { get; set; } // Agregado
        public required string LastName { get; set; } // Agregado
    }
}
