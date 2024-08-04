using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Security;
using System.Security.Authentication;

public class MongoDbService
{
    private readonly MongoClient _client;
    private readonly IMongoDatabase _database;

    public MongoDbService(IConfiguration configuration)
    {
        var connectionUri = configuration["MongoDB:ConnectionString"];
        var databaseName = configuration["MongoDB:DatabaseName"];
        
        var settings = MongoClientSettings.FromConnectionString(connectionUri);
        settings.SslSettings = new SslSettings { EnabledSslProtocols = SslProtocols.Tls12 };
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        _client = new MongoClient(settings);
        _database = _client.GetDatabase(databaseName);
    }

    public void TestConnection()
    {
        try
        {
            var result = _database.RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.ToString()}");
        }
    }

    // Aquí puedes agregar más métodos para interactuar con tu colección de usuarios
}
