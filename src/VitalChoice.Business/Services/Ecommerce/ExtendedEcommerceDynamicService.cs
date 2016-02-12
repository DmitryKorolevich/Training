using System.Data;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Storage;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Options;
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
        protected readonly ITransactionAccessor<EcommerceContext> TransactionAccessor;

        public ExtendedEcommerceDynamicService(IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IEcommerceRepositoryAsync<TEntity> objectRepository,
            IEcommerceRepositoryAsync<TOptionValue> optionValueRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerProviderExtended loggerProvider, DirectMapper<TEntity> directMapper, DynamicExtensionsRewriter queryVisitor, ITransactionAccessor<EcommerceContext> transactionAccessor)
            : base(
                mapper, objectRepository, optionValueRepositoryAsync, bigStringRepository, objectLogItemExternalService, queryVisitor,
                directMapper, loggerProvider.CreateLoggerDefault())
        {
            TransactionAccessor = transactionAccessor;
        }

        protected sealed override IUnitOfWorkAsync CreateUnitOfWork()
        {
            return TransactionAccessor.CreateUnitOfWork();
        }
    }
}