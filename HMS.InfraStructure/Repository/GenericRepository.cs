using HMS.Core.Contracts;
using HMS.Core.Entiites;
using HMS.Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.Repository
{
    public class GenericRepository<Tkey, TEntity> : IGenericRepository<Tkey, TEntity>
        where TEntity : BaseEntity<Tkey>
    {
        private readonly HotelDbContext _dbContext;

        public GenericRepository(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task AddAsync(TEntity entity) 
            => await _dbContext.Set<TEntity>().AddAsync(entity);

        public void Delete(TEntity entity)
            => _dbContext.Set<TEntity>().Remove(entity);

        public async Task<IEnumerable<TEntity>> GetAllAsync()
            => await _dbContext.Set<TEntity>().ToListAsync();

        public async Task<IEnumerable<TEntity>> GetAllAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryExpression = null,
            params Expression<Func<TEntity,object>>[] includeExpressions)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            //IncludeExpression
            if(includeExpressions is not null && includeExpressions.Length > 0)
            {
                foreach(var expression in includeExpressions)
                    query = query.Include(expression);
            }

            //AnyExpression 
            if (queryExpression is not null)
                query = queryExpression(query);

            return await query.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(Tkey id)
            => await _dbContext.Set<TEntity>().FindAsync(id);

        public async Task<TEntity?> GetByIdAsync(Tkey id,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryExpression = null,
            params Expression<Func<TEntity, object>>[] includeExpressions)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();

            if(queryExpression is not null)
                query = queryExpression(query);

            if(includeExpressions is not null && includeExpressions.Length > 0)
            {
                foreach (var expression in includeExpressions)
                    query = query.Include(expression);
            }

           return await query.FirstOrDefaultAsync(x => x.Id!.Equals(id));
        }

        public void Update(TEntity entity)
            => _dbContext.Update(entity);

        public async Task<int> CountAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>>? queryExpression = null)
        {
            var query = _dbContext.Set<TEntity>().AsQueryable();
            if (queryExpression is not null)
                query = queryExpression(query);

            return await query.CountAsync();
        }

    }
}
