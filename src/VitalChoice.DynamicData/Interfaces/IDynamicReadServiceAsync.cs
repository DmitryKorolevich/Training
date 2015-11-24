using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicReadServiceAsync<T, TEntity>
        where TEntity: DynamicDataEntity
        where T: MappedObject
    {
        Task<T> CreatePrototypeAsync(int idObjectType);

        Task<TModel> CreatePrototypeForAsync<TModel>(int idObjectType)
            where TModel : class, new();

        Task<T> SelectAsync(int id, bool withDefaults = false);

        Task<List<T>> SelectAsync(ICollection<int> ids, bool withDefaults = false);

        T Select(int id, bool withDefaults = false);

        List<T> Select(ICollection<int> ids, bool withDefaults = false);

        T CreatePrototype(int idObjectType);

        TModel CreatePrototypeFor<TModel>(int idObjectType)
            where TModel : class, new();
    }
}