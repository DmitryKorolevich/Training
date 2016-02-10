using System.Data;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Business.Services.Ecommerce
{
    public class ExtendedEcommerceDynamicService<TDynamic, TEntity, TOptionType, TOptionValue> :
        DynamicServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        protected readonly EcommerceContext DbContext;

        public ExtendedEcommerceDynamicService(IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IEcommerceRepositoryAsync<TEntity> objectRepository,
            IEcommerceRepositoryAsync<TOptionValue> optionValueRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerProviderExtended loggerProvider, DirectMapper<TEntity> directMapper, DynamicExtensionsRewriter queryVisitor, EcommerceContext dbContext)
            : base(
                mapper, objectRepository, optionValueRepositoryAsync, bigStringRepository, objectLogItemExternalService, queryVisitor,
                directMapper, loggerProvider.CreateLoggerDefault())
        {
            DbContext = dbContext;
        }

        protected sealed override IUnitOfWorkAsync CreateUnitOfWork()
        {
            return new EcommerceUnitOfWork();
        }

        public sealed override IRelationalTransaction BeginTransaction()
        {
            return DbContext.BeginTransaction();
        }
    }
}