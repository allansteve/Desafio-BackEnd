using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MottuMotoRental.Infrastructure.Repositories
{
    public class DeliveryPersonRepository : IDeliveryPersonRepository
    {
        private readonly MotoRentalContext _context;

        public DeliveryPersonRepository(MotoRentalContext context)
        {
            _context = context;
        }

        public async Task<DeliveryPerson> GetByIdAsync(int id)
        {
            return await _context.DeliveryPersons.FindAsync(id);
        }

        public async Task AddAsync(DeliveryPerson deliveryPerson)
        {
            await _context.DeliveryPersons.AddAsync(deliveryPerson);
            await _context.SaveChangesAsync();
        }

        public async Task<DeliveryPerson> GetByCnpjAsync(string cnpj)
        {
            return await _context.DeliveryPersons.FirstOrDefaultAsync(dp => dp.Cnpj == cnpj);
        }

        public async Task<DeliveryPerson> GetByDriverLicenseNumberAsync(string driverLicenseNumber)
        {
            return await _context.DeliveryPersons.FirstOrDefaultAsync(dp =>
                dp.DriverLicenseNumber == driverLicenseNumber);
        }
    }
}