using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;

namespace MottuMotoRental.Infrastructure.Data
{
    public class MongoDbContext : IMongoDbContext
    {
        private readonly IMongoDatabase _database;

        public MongoDbContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDbConnection"));
            _database = client.GetDatabase("MottuRentalMongoDB");
        }

        public IMongoCollection<Motorcycle> Motorcycles => _database.GetCollection<Motorcycle>("RegisteredMotorcycles");
    }
}