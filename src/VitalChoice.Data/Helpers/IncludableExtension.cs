using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Domain;

namespace VitalChoice.Data.Helpers
{
    public static class IncludableExtension
    {
        public static IIncludableQueryFluent<TEntity, TProperty> ThenInclude<TProperty, TEntity, TPreviousProperty>(
            this IIncludableQueryFluent<TEntity, TPreviousProperty> previous,
            Expression<Func<TPreviousProperty, TProperty>> expression)
            where TEntity : Entity
        {
            return new IncludableQueryFluent<TEntity, TProperty>(previous,
                ((IncludableQueryFluent<TEntity, TPreviousProperty>) previous).IncludableQuery.ThenInclude(expression));
        }

        public static IIncludableQueryFluent<TEntity, TProperty> ThenInclude<TProperty, TEntity, TPreviousProperty>(
            this IIncludableQueryFluent<TEntity, ICollection<TPreviousProperty>> previous,
            Expression<Func<TPreviousProperty, TProperty>> expression)
            where TEntity : Entity
        {
            return new IncludableQueryFluent<TEntity, TProperty>(previous,
                ((IncludableQueryFluent<TEntity, ICollection<TPreviousProperty>>) previous).IncludableQuery.ThenInclude(
                    expression));
        }
    }
}