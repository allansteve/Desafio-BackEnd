using System.Threading.Tasks;
using MottuMotoRental.Core.Entities;

namespace MottuMotoRental.Core.Interfaces
{
    public interface IMotorcycleRepository
    {
        Task<Motorcycle> GetByLicensePlateAsync(string licensePlate);
        Task AddAsync(Motorcycle motorcycle);
        Task<IEnumerable<Motorcycle>> GetAllAsync();
        Task<Motorcycle> GetByIdAsync(int id);
        Task UpdateAsync(Motorcycle motorcycle);
        Task DeleteAsync(int id);
        Task<bool> HasActiveRentalsAsync(int motorcycleId);
    }
}