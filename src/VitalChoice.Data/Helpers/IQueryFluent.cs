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
    public interface IQueryFluent<TEntity> : IQueryBaseFluent<TEntity> where TEntity : Entity
    {
        IQueryIncludeFluent<TEntity, TProperty> IncludeNew<TProperty>(Expression<Func<TEntity, TProperty>> expression) where TProperty : Entity;
    }
}