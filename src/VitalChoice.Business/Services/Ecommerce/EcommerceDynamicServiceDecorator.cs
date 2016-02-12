using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Storage;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Business.Services.Ecommerce
{
    public class EcommerceDynamicServiceDecorator<TDynamic, TEntity> : EcommerceDynamicReadServiceDecorator<TDynamic, TEntity>,
        IDynamicServiceAsync<TDynamic, TEntity>
        where TEntity : DynamicDataEntity, new()
        where TDynamic : MappedObject, new()
    {
        private readonly IDynamicServiceAsync<TDynamic, TEntity> _extendedService;

        public EcommerceDynamicServiceDecorator(IDynamicServiceAsync<TDynamic, TEntity> extendedService) : base(extendedService)
        {
            _extendedService = extendedService;
        }

        public Task<TDynamic> InsertAsync(TDynamic model)
        {
            return _extendedService.InsertAsync(model);
        }

        public Task<TDynamic> UpdateAsync(TDynamic model)
        {
            return _extendedService.UpdateAsync(model);
        }

        public Task<List<TDynamic>> InsertRangeAsync(ICollection<TDynamic> models)
        {
            return _extendedService.InsertRangeAsync(models);
        }

        public Task<List<TDynamic>> UpdateRangeAsync(ICollection<TDynamic> models)
        {
            return _extendedService.UpdateRangeAsync(models);
        }

        public Task<bool> DeleteAllAsync(ICollection<TDynamic> models, bool physically = false)
        {
            return _extendedService.DeleteAllAsync(models, physically);
        }

        public Task<bool> DeleteAsync(TDynamic model, bool physically = false)
        {
            return _extendedService.DeleteAsync(model, physically);
        }

        public Task<bool> DeleteAllAsync(ICollection<int> list, bool physically = false)
        {
            return _extendedService.DeleteAllAsync(list, physically);
        }

        public Task<bool> DeleteAsync(int id, bool physically = false)
        {
            return _extendedService.DeleteAsync(id, physically);
        }

        public TDynamic Insert(TDynamic model)
        {
            return _extendedService.Insert(model);
        }

        public TDynamic Update(TDynamic model)
        {
            return _extendedService.Update(model);
        }

        public List<TDynamic> InsertRange(ICollection<TDynamic> models)
        {
            return _extendedService.InsertRange(models);
        }

        public List<TDynamic> UpdateRange(ICollection<TDynamic> models)
        {
            return _extendedService.UpdateRange(models);
        }

        public bool DeleteAll(ICollection<TDynamic> models, bool physically = false)
        {
            return _extendedService.DeleteAll(models, physically);
        }

        public bool Delete(TDynamic model, bool physically = false)
        {
            return _extendedService.Delete(model, physically);
        }

        public bool DeleteAll(ICollection<int> list, bool physically = false)
        {
            return _extendedService.DeleteAll(list, physically);
        }

        public bool Delete(int id, bool physically = false)
        {
            return _extendedService.Delete(id, physically);
        }
    }
}