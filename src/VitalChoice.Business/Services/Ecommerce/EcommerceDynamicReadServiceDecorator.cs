using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Business.Services.Ecommerce
{
    public class EcommerceDynamicReadServiceDecorator<TDynamic, TEntity> :
        IDynamicReadServiceAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity
        where TDynamic : MappedObject
    {
        private readonly IDynamicReadServiceAsync<TDynamic, TEntity> _extendedReadService;

        public EcommerceDynamicReadServiceDecorator(IDynamicReadServiceAsync<TDynamic, TEntity> extendedService)
        {
            _extendedReadService = extendedService;
        }

        public Task<TDynamic> CreatePrototypeAsync(int idObjectType)
        {
            return _extendedReadService.CreatePrototypeAsync(idObjectType);
        }

        public Task<TModel> CreatePrototypeForAsync<TModel>(int idObjectType) where TModel : class, new()
        {
            return _extendedReadService.CreatePrototypeForAsync<TModel>(idObjectType);
        }

        public Task<TDynamic> SelectAsync(int id, bool withDefaults = false)
        {
            return _extendedReadService.SelectAsync(id, withDefaults);
        }

        public Task<List<TDynamic>> SelectAsync(ICollection<int> ids, bool withDefaults = false)
        {
            return _extendedReadService.SelectAsync(ids, withDefaults);
        }

        public TDynamic Select(int id, bool withDefaults = false)
        {
            return _extendedReadService.Select(id, withDefaults);
        }

        public List<TDynamic> Select(ICollection<int> ids, bool withDefaults = false)
        {
            return _extendedReadService.Select(ids, withDefaults);
        }

        public TDynamic CreatePrototype(int idObjectType)
        {
            return _extendedReadService.CreatePrototype(idObjectType);
        }

        public TModel CreatePrototypeFor<TModel>(int idObjectType) where TModel : class, new()
        {
            return _extendedReadService.CreatePrototypeFor<TModel>(idObjectType);
        }
    }
}