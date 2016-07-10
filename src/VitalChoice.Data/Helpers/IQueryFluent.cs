using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Transfer;

namespace VitalChoice.Data.Helpers
{

    public interface IQueryFluent<TEntity>
        where TEntity : Entity
    {
        IIncludableQueryFluent<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression);

        IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        Task<bool> SelectAnyAsync();
        Task<int> SelectCountAsync();
        Task<decimal> SelectSumAsync(Expression<Func<TEntity, decimal>> func);
        List<TEntity> SelectPage(int page, int pageSize, out int totalCount, bool tracking = false);
        Task<PagedList<TEntity>> SelectPageAsync(int page, int pageSize, bool tracking = false);
        List<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector, bool tracking);
        List<TEntity> Select(bool tracking);

        TEntity SelectFirstOrDefault(bool tracking);
        Task<List<TEntity>> SelectAsync(bool tracking);
        Task<List<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> selector, bool tracking);
        Task<TEntity> SelectFirstOrDefaultAsync(bool tracking);
        IQueryFluent<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IQueryFluent<TEntity> Distinct();
    }
}