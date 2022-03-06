using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DeviceManager.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        void DetachAllEntities();
        void BeginTransaction();
        Task<bool> CommitTransactionAsync();
        bool CommitTransaction();
        void RollbackTransaction();
        Task<bool> SaveAsync();
        bool Save();
        bool Remove(TEntity entity);
        bool Remove(object id);
        bool InsertRange(IEnumerable<TEntity> entity);
        Task<bool> InsertRangeAsync(IEnumerable<TEntity> entity);
        bool Insert(TEntity entity);
        Task<bool> InsertAsync(TEntity entity);
        bool Update(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        bool Delete(TEntity entity);
        Task<bool> DeleteAsync(TEntity entity);
        bool Delete(object id);
        Task<bool> DeleteAsync(Expression<Func<TEntity, bool>> filter);
        bool Delete(Expression<Func<TEntity, bool>> filter);
        Task<bool> DeleteAsync(object id);
        int Count();
        int Count(Expression<Func<TEntity, bool>> filter);
        Task<int> CountAsync();
        Task<int> CountAsync(Expression<Func<TEntity, bool>> filter);
        TEntity Get(Expression<Func<TEntity, bool>> filter);
        TEntity GetAsNoTracking(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> filter = null);
        TEntity LastOrDefault(Expression<Func<TEntity, bool>> filter = null);

        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> filter);
        Task<IQueryable<TEntity>> GetAllAsyncQueryable();
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IQueryable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> filter);
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        Task<TEntity> GetAsyncAsNoTracking(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includeProperties);
        TEntity GetById(object id);
        Task<TEntity> GetByIdAsync(object id);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetAllListAsync();
        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> FirstOrDefaultAsync(int id);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate);
        TEntity Add(TEntity entity);
        Task<TEntity> AddAsync(TEntity entity);
        int CountAll();
        Task<int> CountAllAsync();
        int CountAll(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAllAsync(Expression<Func<TEntity, bool>> predicate);
    }

}
