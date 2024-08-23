using MongoDB.Driver;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MottuMotoRental.Tests.Data
{
    public class FakeMongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public FakeMongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase(configuration["DatabaseName"]);
        }

        public IMongoCollection<Motorcycle> Motorcycles => _database.GetCollection<Motorcycle>("Motorcycles");
    }
}