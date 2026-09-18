using HMS.Core.Entiites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Contracts
{
    public interface IGenericRepository<Tkey,TEntity>
        where TEntity : BaseEntity<Tkey>
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryExpression = null,
            params Expression<Func<TEntity, object>>[] includeExpressions);

        Task<TEntity?> GetByIdAsync(Tkey id);
        Task<TEntity?> GetByIdAsync(Tkey id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryExpression = null,
            params Expression<Func<TEntity, object>>[] includeExpressions);

        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        Task<int> CountAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryExpression = null);
    }
}
