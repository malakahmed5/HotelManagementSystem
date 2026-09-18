using HMS.Core.Entiites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Core.Contracts
{
    public interface IUnitOfWork
    {
        IGenericRepository<Tkey, TEntity> GetRepository<Tkey, TEntity>()
            where TEntity : BaseEntity<Tkey>;

        Task<int> SaveChangesAsync();
    }
}
