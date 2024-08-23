using MottuMotoRental.Core.Entities;
using MottuMotoRental.Core.Interfaces;
using MottuMotoRental.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace MottuMotoRental.Infrastructure.Repositories
{
    public class RentalRepository : IRentalRepository
    {
        private readonly MotoRentalContext _context;

        public RentalRepository(MotoRentalContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Rental rental)
        {
            await _context.Rentals.AddAsync(rental);
            await _context.SaveChangesAsync();
        }

        public async Task<Rental> GetByIdAsync(int id)
        {
            return await _context.Rentals.FindAsync(id);
        }
        
        public async Task UpdateAsync(Rental rental)
        {
            _context.Rentals.Update(rental);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasActiveRentalAsync(int deliveryPersonId)
        {
            return await _context.Rentals.AnyAsync(r => r.DeliveryPersonId == deliveryPersonId && r.IsActive);
        }
    }
}