#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

#endregion

namespace VitalChoice.Data.Helpers
{
    public interface IQueryBaseFluent<TEntity> where TEntity : Entity
    {
        IQueryFluent<TEntity> OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy);
        IQueryFluent<TEntity> Include(Expression<Func<TEntity, object>> expression);
        IEnumerable<TEntity> SelectPage(int page, int pageSize, out int totalCount, bool tracking = true);
        IEnumerable<TResult> Select<TResult>(Expression<Func<TEntity, TResult>> selector = null, bool tracking = true);
        IEnumerable<TEntity> Select(bool tracking = true);
        Task<IEnumerable<TEntity>> SelectAsync(bool tracking = true);
    }
}