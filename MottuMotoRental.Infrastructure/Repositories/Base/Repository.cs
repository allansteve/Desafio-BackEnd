using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using MottuMotoRental.Infrastructure.Data;
using System.Linq.Expressions;

namespace MottuMotoRental.Infrastructure.Repositories.Base
{
    public interface IRepository<T>
    {
        IQueryable<T> Query();

        IQueryable<U> SelectQuery<U>(Expression<Func<T, U>> selector);

        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);

        T Insert(T entity, bool save = true);

        T Modify(T entity, bool save = true);

        T Update(T entity, bool save = true);

        void Delete(T entity, bool save = true);

        Task<T> InsertAsync(T entity, bool save = true);

        Task<IEnumerable<T>> InsertAllAsync(IEnumerable<T> entities, bool save = true);

        Task<T> ModifyAsync(T entity, bool save = true);

        Task<T> PatchAsync(T entity, params Expression<Func<T, object?>>[] propertyExpressions);

        Task<T> UpdateAsync(T entity, bool save = true);

        Task DeleteAsync(T entity, bool save = true);

        Task<bool> ExecuteDeleteAsync(Expression<Func<T, bool>> whereFn);

        Task DeleteAllAsync(IEnumerable<T> entities, bool save = true);

        Task<int> ExecuteUpdateAsync(
            Expression<Func<T, bool>> whereFn,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls,
            CancellationToken ct = default
        );

        Task SaveChangesAsync();

        void ModifyProps(T entity, params Expression<Func<T, object?>>[] propertyExpressions);
    }

    public abstract class Repository<T> : IRepository<T>
        where T : class, new()
    {
        protected readonly MotoRentalContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(MotoRentalContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual IQueryable<T> Query() => _dbSet.AsQueryable().AsSplitQuery();

        public IQueryable<U> SelectQuery<U>(Expression<Func<T, U>> selector) =>
            Query().Select(selector);

        public virtual async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate) =>
            await Query().AnyAsync(predicate);

        public virtual async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

        public T Insert(T entity, bool save = true)
        {
            _dbSet.Add(entity);

            if (save)
                SaveChanges();

            return entity;
        }

        public async Task<T> InsertAsync(T entity, bool save = true)
        {
            await _dbSet.AddAsync(entity);

            if (save)
                await SaveChangesAsync();

            return entity;
        }

        public async Task<IEnumerable<T>> InsertAllAsync(IEnumerable<T> entities, bool save = true)
        {
            await _dbSet.AddRangeAsync(entities);

            if (save)
                await SaveChangesAsync();

            return entities;
        }

        public T Modify(T entity, bool save = true)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;

            if (save)
                SaveChanges();

            return entity;
        }

        public T Update(T entity, bool save = true)
        {
            _dbSet.Update(entity);

            if (save)
                SaveChanges();

            return entity;
        }

        public void Delete(T entity, bool save = true)
        {
            _dbSet.Remove(entity);

            if (save)
                SaveChanges();
        }

        public async Task<T> ModifyAsync(T entity, bool save = true)
        {
            _dbSet.Entry(entity).State = EntityState.Modified;

            if (save)
                await SaveChangesAsync();

            return entity;
        }

        public async Task<T> PatchAsync(
            T entity,
            params Expression<Func<T, object?>>[] propertyExpressions
        )
        {
            ModifyProps(entity, propertyExpressions);
            await SaveChangesAsync();
            return entity;
        }

        public async Task<T> UpdateAsync(T entity, bool save = true)
        {
            _dbSet.Update(entity);

            if (save)
                await SaveChangesAsync();

            return entity;
        }

        public async Task DeleteAsync(T entity, bool save = true)
        {
            _dbSet.Remove(entity);

            if (save)
                await SaveChangesAsync();
        }

        public async Task<int> ExecuteUpdateAsync(
            Expression<Func<T, bool>> whereFn,
            Expression<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>> setPropertyCalls,
            CancellationToken ct = default
        ) => await _dbSet.Where(whereFn).ExecuteUpdateAsync(setPropertyCalls, ct);

        public async Task<bool> ExecuteDeleteAsync(Expression<Func<T, bool>> whereFn)
        {
            var deleteCount = await _dbSet.Where(whereFn).ExecuteDeleteAsync();

            return deleteCount > 0;
        }

        public async Task DeleteAllAsync(IEnumerable<T> entities, bool save = true)
        {
            _dbSet.RemoveRange(entities);

            if (save)
                await SaveChangesAsync();
        }

        public void SaveChanges() => _context.SaveChanges();

        public async Task SaveChangesAsync() => await _context.SaveChangesAsync();


        public void ModifyProps(T entity, params Expression<Func<T, object?>>[] propertyExpressions)
        {
            foreach (var propertyExpression in propertyExpressions)
            {
                _dbSet.Entry(entity).Property(propertyExpression).IsModified = true;
            }
        }
    }
}
