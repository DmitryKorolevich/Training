#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Base;
using VitalChoice.Domain.Infrastructure;

#endregion

namespace VitalChoice.Data.Helpers
{
    public interface IQueryFluent<TEntity> where TEntity : Entity
    {
        IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        IIncludableQueryFluent<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty>> expression);
        IIncludableQueryFluent<TEntity, TProperty> Include<TProperty>(Expression<Func<TEntity, ICollection<TProperty>>> expression);
        Task<bool> SelectAnyAsync();
        Task<int> SelectCountAsync();
        IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount, bool tracking = true);
        Task<PagedList<TEntity>> SelectPageAsync(int page, int pageSize, bool tracking = false);
        IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector = null, bool tracking = true);
        IEnumerable<TEntity> Select(bool tracking = true);
        Task<IEnumerable<TEntity>> SelectAsync(bool tracking = true);
    }
}