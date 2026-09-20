using HMS.Core.Contracts;
using HMS.Core.Entiites;
using HMS.Infrastructure.Data.DbContexts;
using HMS.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Infrastructure.UnirOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelDbContext _dbContext;
        private readonly Dictionary<Type, object> _repositories = [];
        public UnitOfWork(HotelDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IGenericRepository<Tkey, TEntity> GetRepository<Tkey, TEntity>() where TEntity : BaseEntity<Tkey>
        {
            var type = typeof(TEntity);
            if (_repositories.TryGetValue(type, out var repository))
                return (IGenericRepository<Tkey, TEntity>)repository;

            var newRepo = new GenericRepository<Tkey, TEntity>(_dbContext);
            _repositories[type] = newRepo;

            return newRepo;
        }

        public async Task<int> SaveChangesAsync()
            => await _dbContext.SaveChangesAsync();
    }
}
