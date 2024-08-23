using MottuMotoRental.Infrastructure.Data;

namespace MottuMotoRental.Infrastructure.Repositories.Base
{
    public interface IUnitOfWork
    {
        void Save();


        bool BeginTransaction();

        void RollBack();

        void Commit();

        Task SaveAsync();

        Task CommitAsync();

        MotoRentalContext GetContext();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly MotoRentalContext _ctx;

        public UnitOfWork(MotoRentalContext dbContext)
        {
            _ctx = dbContext;
        }

        public void Save()
        {
            _ctx.SaveChanges();
        }

        public Task SaveAsync()
        {
            return _ctx.SaveChangesAsync();
        }

        public bool BeginTransaction()
        {
            if (_ctx.Database.CurrentTransaction is not null)
                return false;
            _ctx.Database.BeginTransaction();
            return true;
        }

        public void Commit()
        {
            if (_ctx.Database.CurrentTransaction is null)
                return;
            _ctx.Database.CommitTransaction();
        }

        public Task CommitAsync()
        {
            if (_ctx.Database.CurrentTransaction is null)
                return Task.CompletedTask;

            return _ctx.Database.CommitTransactionAsync();
        }

        public void RollBack()
        {
            if (_ctx.Database.CurrentTransaction is null)
                return;

            _ctx.Database.RollbackTransaction();
        }

        public void RollBackAsync()
        {
            if (_ctx.Database.CurrentTransaction is null)
                return;
            _ctx.Database.RollbackTransactionAsync();
        }

        public MotoRentalContext GetContext()
        {
            return _ctx;
        }
    }
}
