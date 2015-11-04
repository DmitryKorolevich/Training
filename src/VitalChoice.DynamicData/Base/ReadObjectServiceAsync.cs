using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Base
{
    //public abstract partial class DynamicReadServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>
    //    where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
    //    where TOptionType : OptionType, new()
    //    where TOptionValue : OptionValue<TOptionType>, new()
    //    where TDynamic : MappedObject, new()
    //{
    //    public Task<TDynamic> SelectAsync(int id)
    //    {
    //        return SelectAsync(id, false);
    //    }

    //    public Task<List<TDynamic>> SelectAsync(ICollection<int> ids)
    //    {
    //        return SelectAsync(ids, false);
    //    }

    //    public Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject = null,
    //        IDictionary<string, object> values = null,
    //        Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
    //    {
    //        return SelectAsync(queryObject, values, includesOverride, false);
    //    }

    //    public Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query = null,
    //        IDictionary<string, object> values = null,
    //        Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
    //    {
    //        return SelectAsync(query, values, includesOverride, false);
    //    }

    //    public TDynamic Select(int id)
    //    {
    //        return Select(id, false);
    //    }

    //    public List<TDynamic> Select(ICollection<int> ids)
    //    {
    //        return Select(ids, false);
    //    }

    //    public List<TDynamic> Select(IQueryObject<TEntity> queryObject = null, IDictionary<string, object> values = null,
    //        Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
    //    {
    //        return Select(queryObject, values, includesOverride, false);
    //    }

    //    public List<TDynamic> Select(Expression<Func<TEntity, bool>> query = null,
    //        IDictionary<string, object> values = null,
    //        Func<IQueryLite<TEntity>, IQueryLite<TEntity>> includesOverride = null)
    //    {
    //        return Select(query, values, includesOverride, false);
    //    }
    //}
}
