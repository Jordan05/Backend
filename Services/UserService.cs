using MongoDB.Bson;
using MongoDB.Driver;
using MyApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyApi.Services
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config["MongoDB:ConnectionString"]);
            var database = client.GetDatabase(config["MongoDB:DatabaseName"]);
            _users = database.GetCollection<User>(config["MongoDB:UserCollectionName"]);
        }

        public async Task<List<User>> GetAsync() =>
            await _users.Find(user => true).ToListAsync();

        public async Task<User> GetAsync(ObjectId id) =>
            await _users.Find(user => user.Id == id).FirstOrDefaultAsync();

        public async Task<User> GetByEmailAsync(string email) =>
            await _users.Find(user => user.Email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(User newUser) =>
            await _users.InsertOneAsync(newUser);

        public async Task UpdateAsync(ObjectId id, User updatedUser) =>
            await _users.ReplaceOneAsync(user => user.Id == id, updatedUser);

        public async Task RemoveAsync(ObjectId id) =>
            await _users.DeleteOneAsync(user => user.Id == id);
    }
}
