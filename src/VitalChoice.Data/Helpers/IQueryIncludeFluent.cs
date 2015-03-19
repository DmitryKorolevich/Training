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
    public interface IQueryIncludeFluent<TEntity, TProperty> : IQueryBaseFluent<TEntity> where TEntity: Entity
                                                                                         where TProperty : Entity

    {
        IQueryIncludeFluent<TProperty, TNewProperty> SubInclude<TNewProperty>(Expression<Func<TProperty, TNewProperty>> expression) where TNewProperty : Entity;
    }
}