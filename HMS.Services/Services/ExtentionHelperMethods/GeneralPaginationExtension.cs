using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HMS.Services.Services.ExtentionHelperMethods
{
    public static class GeneralPaginationExtension
    {
        public static IQueryable<TEntity> ApplyPagination<TEntity>(this IQueryable<TEntity> query , int pageSize , int pageIndex)
        {
            int skip = (pageIndex - 1) * pageSize;
            int take = pageSize;

            return query.Skip(skip).Take(take);
        }
    }
}
