#region

using System;
using System.Linq.Expressions;
using VitalChoice.Data.Extensions;

#endregion

namespace VitalChoice.Data.Helpers
{
    public abstract class QueryObject<TEntity> : IQueryObject<TEntity>
    {
        private Expression<Func<TEntity, bool>> query;
        public virtual Expression<Func<TEntity, bool>> Query()
        {
            return query;
        }

        public Expression<Func<TEntity, bool>> And(Expression<Func<TEntity, bool>> query)
        {
            return this.query == null ? query : this.query.And(query.Expand());
        }

        public Expression<Func<TEntity, bool>> Or(Expression<Func<TEntity, bool>> query)
        {
            return this.query == null ? query : this.query.Or(query.Expand());
        }

        public Expression<Func<TEntity, bool>> And(IQueryObject<TEntity> queryObject)
        {
            return And(queryObject.Query());
        }

        public Expression<Func<TEntity, bool>> Or(IQueryObject<TEntity> queryObject)
        {
            return Or(queryObject.Query());
        }

        protected void Add(Expression<Func<TEntity, bool>> predicate)
        {
            query = (query == null) ? predicate : query.And(predicate.Expand());
        }
    }
}