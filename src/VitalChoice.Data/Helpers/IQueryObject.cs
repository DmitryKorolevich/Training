using System;
using System.Linq.Expressions;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Data.Helpers
{
    public interface IQueryOptionType<TOptionType> : IQueryObject<TOptionType>
        where TOptionType : OptionType
    {
        IQueryOptionType<TOptionType> WithObjectType(int? objectType);
    }

    public interface IQueryObject<TEntity>
    {
        Expression<Func<TEntity, bool>> Query();
        Expression<Func<TEntity, bool>> And(Expression<Func<TEntity, bool>> query);
        Expression<Func<TEntity, bool>> Or(Expression<Func<TEntity, bool>> query);
        Expression<Func<TEntity, bool>> And(IQueryObject<TEntity> queryObject);
        Expression<Func<TEntity, bool>> Or(IQueryObject<TEntity> queryObject);
    }
}