using MongoDB.Driver;
using MottuMotoRental.Core.Entities;

namespace MottuMotoRental.Core.Interfaces
{
    public interface IMongoDbContext
    {
        IMongoCollection<Motorcycle> Motorcycles { get; }
    }
}