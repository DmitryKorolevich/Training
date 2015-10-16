using System;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.eCommerce;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.History;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class EcommerceDynamicObjectService<TDynamic, TEntity, TOptionType, TOptionValue> :
        DynamicObjectServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>,
        IEcommerceDynamicObjectService<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        public EcommerceDynamicObjectService(IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IEcommerceRepositoryAsync<TEntity> objectRepository,
            IEcommerceRepositoryAsync<TOptionType> optionTypesRepository,
            IEcommerceRepositoryAsync<TOptionValue> optionValueRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepository,
            IEcommerceRepositoryAsync<ObjectHistoryLogItem> objectHistoryLogItemRepository)
            : base(mapper, objectRepository, optionTypesRepository, optionValueRepositoryAsync, bigStringRepository, objectHistoryLogItemRepository)
        {
        }

        private ObjectType? _idObjectType;

        public override ObjectType IdObjectType
        {
            get
            {
                if (!_idObjectType.HasValue)
                {
                    var result = ObjectType.Unknown;
                    Enum.TryParse<ObjectType>(typeof(TEntity).Name, out result);
                    _idObjectType = result;
                }
                return _idObjectType.Value;
            }
        }

        protected override sealed IUnitOfWorkAsync CreateUnitOfWork()
        {
            return new EcommerceUnitOfWork();
        }
    }
}