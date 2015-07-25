using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Base
{
    public abstract partial class ReadDynamicObjectServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        public async Task<TDynamic> SelectAsync(int id)
        {
            return await SelectAsync(id, false);
        }

        public async Task<List<TDynamic>> SelectAsync(ICollection<int> ids)
        {
            return await SelectAsync(ids, false);
        }

        public async Task<List<TDynamic>> SelectAsync()
        {
            return await SelectAsync(false);
        }

        public async Task<List<TDynamic>> SelectAsync(IQueryObject<TEntity> queryObject)
        {
            return await SelectAsync(queryObject, false);
        }

        public async Task<List<TDynamic>> SelectAsync(Expression<Func<TEntity, bool>> query)
        {
            return await SelectAsync(query, false);
        }

        public async Task<List<TDynamic>> SelectAsync(IDictionary<string, object> values)
        {
            return await SelectAsync(values, false);
        }

        public async Task<List<TDynamic>> SelectAsync(IDictionary<string, object> values, Expression<Func<TEntity, bool>> query)
        {
            return await SelectAsync(values, query, false);
        }

        public TDynamic Select(int id)
        {
            return Select(id, false);
        }

        public List<TDynamic> Select(ICollection<int> ids)
        {
            return Select(ids, false);
        }

        public List<TDynamic> Select()
        {
            return Select(false);
        }

        public List<TDynamic> Select(IQueryObject<TEntity> queryObject)
        {
            return Select(queryObject, false);
        }

        public List<TDynamic> Select(Expression<Func<TEntity, bool>> query)
        {
            return Select(query, false);
        }

        public List<TDynamic> Select(IDictionary<string, object> values)
        {
            return Select(values, false);
        }

        public List<TDynamic> Select(IDictionary<string, object> values, Expression<Func<TEntity, bool>> query)
        {
            return Select(values, query, false);
        }
    }
}
