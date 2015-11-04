using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Domain;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Data.Helpers
{

    public interface IQueryFluent<TEntity>
        where TEntity : Entity
    {
        IIncludableQueryFluent<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression);
        IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        Task<bool> SelectAnyAsync();
        Task<int> SelectCountAsync();
        List<TEntity> SelectPage(int page, int pageSize, out int totalCount, bool tracking = true);
        Task<PagedList<TEntity>> SelectPageAsync(int page, int pageSize, bool tracking = false);
        List<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector = null, bool tracking = true);
        List<TEntity> Select(bool tracking = true);
        Task<List<TEntity>> SelectAsync(bool tracking = true);
        Task<List<TResult>> SelectAsync<TResult>(Expression<Func<TEntity, TResult>> selector, bool tracking = true);
        Task<TEntity> SelectFirstOrDefaultAsync(bool tracking = true);
        IQueryFluent<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        IQueryFluent<TEntity> Distinct();
    }
}