using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MottuMotoRental.Core.Entities;

namespace MottuMotoRental.Infrastructure.Data
{
    public class MotoRentalContext : IdentityDbContext<SystemUser, SystemRole, Guid>
    {
        public MotoRentalContext(DbContextOptions<MotoRentalContext> options) : base(options) { }

        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<SystemUser> SystemUsers{ get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Motorcycle>().HasIndex(m => m.LicensePlate).IsUnique();
            modelBuilder.Entity<DeliveryPerson>().HasIndex(d => d.Cnpj).IsUnique();
            modelBuilder.Entity<DeliveryPerson>().HasIndex(d => d.DriverLicenseNumber).IsUnique();

            
            modelBuilder.Entity<Rental>()
                .HasOne<DeliveryPerson>()
                .WithMany()
                .HasForeignKey(r => r.DeliveryPersonId);

            modelBuilder.Entity<Rental>()
                .HasOne<Motorcycle>()
                .WithMany()
                .HasForeignKey(r => r.MotorcycleId);

            base.OnModelCreating(modelBuilder);
        }
    }
}