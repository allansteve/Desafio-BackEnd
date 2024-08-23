using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MottuMotoRental.Infrastructure.Repositories
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly MotoRentalContext _context;

        public MotorcycleRepository(MotoRentalContext context)
        {
            _context = context;
        }

        public async Task<Motorcycle> GetByLicensePlateAsync(string licensePlate)
        {
            return await _context.Motorcycles.FirstOrDefaultAsync(m => m.LicensePlate == licensePlate);
        }

        public async Task AddAsync(Motorcycle motorcycle)
        {
            await _context.Motorcycles.AddAsync(motorcycle);
            await _context.SaveChangesAsync();
        }

        public async Task<Motorcycle> GetByIdAsync(int id)
        {
            return await _context.Motorcycles.FindAsync(id);
        }

        public async Task<IEnumerable<Motorcycle>> GetAllAsync()
        {
            return await _context.Motorcycles.ToListAsync();
        }

        public async Task UpdateAsync(Motorcycle motorcycle)
        {
            _context.Motorcycles.Update(motorcycle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var motorcycle = await _context.Motorcycles.FindAsync(id);
            if (motorcycle != null)
            {
                _context.Motorcycles.Remove(motorcycle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> HasActiveRentalsAsync(int motorcycleId)
        {
            return await _context.Rentals
                .AnyAsync(r => r.MotorcycleId == motorcycleId && r.IsActive);
        }
    }
}