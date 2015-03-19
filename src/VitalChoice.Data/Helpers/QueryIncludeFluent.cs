#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

#endregion

namespace VitalChoice.Data.Helpers
{
	public sealed class QueryIncludeFluent<TEntity, TProperty> : IQueryIncludeFluent<TEntity, TProperty> where TEntity : Entity
                                                                                                         where TProperty : Entity
    {
        private readonly IQueryBaseFluent<TEntity> baseQueryFluent;

        public Expression<Func<TEntity, TProperty>> expression { get; private set; }

        public IQueryIncludeFluent<TProperty, Entity> Next { get; private set; }

        public QueryIncludeFluent(IQueryBaseFluent<TEntity> baseQueryFluent, Expression<Func<TEntity, TProperty>> expression)
        {
            this.baseQueryFluent = baseQueryFluent;
            this.expression = expression;
        }

        public IQueryIncludeFluent<TProperty, TNewProperty> SubInclude<TNewProperty>(Expression<Func<TProperty, TNewProperty>> expression) where TNewProperty : Entity
        {
            throw new NotImplementedException();
        }

        IQueryFluent<TEntity> IQueryBaseFluent<TEntity>.OrderBy(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy)
        {
            return baseQueryFluent.OrderBy(orderBy);
        }

        IEnumerable<TEntity> IQueryBaseFluent<TEntity>.SelectPage(int page, int pageSize, out int totalCount, bool tracking)
        {
            return baseQueryFluent.SelectPage(page, pageSize, out totalCount, tracking);
        }

        IEnumerable<TResult> IQueryBaseFluent<TEntity>.Select<TResult>(Expression<Func<TEntity, TResult>> selector, bool tracking)
        {
            return baseQueryFluent.Select<TResult>(selector, tracking);
        }

        IEnumerable<TEntity> IQueryBaseFluent<TEntity>.Select(bool tracking)
        {
            return baseQueryFluent.Select(tracking);
        }

        Task<IEnumerable<TEntity>> IQueryBaseFluent<TEntity>.SelectAsync(bool tracking)
        {
            return baseQueryFluent.SelectAsync(tracking);
        }

        public IQueryFluent<TEntity> Include(Expression<Func<TEntity, object>> expression)
        {
            throw new NotImplementedException();
        }
    }
}