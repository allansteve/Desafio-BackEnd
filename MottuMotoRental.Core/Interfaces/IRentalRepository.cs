using MottuMotoRental.Core.Entities;
using System.Threading.Tasks;

namespace MottuMotoRental.Core.Interfaces
{
    public interface IRentalRepository
    {
        Task AddAsync(Rental rental);
        Task<Rental> GetByIdAsync(int id);
        Task UpdateAsync(Rental rental);
        Task<bool> HasActiveRentalAsync(int deliveryPersonId);
    }
}