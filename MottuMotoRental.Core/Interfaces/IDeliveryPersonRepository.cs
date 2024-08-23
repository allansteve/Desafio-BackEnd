using MottuMotoRental.Core.Entities;
using System.Threading.Tasks;

namespace MottuMotoRental.Core.Interfaces
{
    public interface IDeliveryPersonRepository
    {
        Task<DeliveryPerson> GetByIdAsync(int id);
        Task AddAsync(DeliveryPerson deliveryPerson);
        Task<DeliveryPerson> GetByCnpjAsync(string cnpj);
        Task<DeliveryPerson> GetByDriverLicenseNumberAsync(string driverLicenseNumber);
    }
}