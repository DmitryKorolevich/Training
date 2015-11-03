﻿using System;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Services;
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
    public class EcommerceDynamicService<TDynamic, TEntity, TOptionType, TOptionValue> :
        DynamicServiceAsync<TDynamic, TEntity, TOptionType, TOptionValue>,
        IEcommerceDynamicService<TDynamic, TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject, new()
    {
        public EcommerceDynamicService(IDynamicMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper,
            IEcommerceRepositoryAsync<TEntity> objectRepository,
            IEcommerceRepositoryAsync<TOptionType> optionTypesRepository,
            IEcommerceRepositoryAsync<TOptionValue> optionValueRepositoryAsync,
            IEcommerceRepositoryAsync<BigStringValue> bigStringRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerProviderExtended loggerProvider)
            : base(mapper, objectRepository, optionTypesRepository, optionValueRepositoryAsync, bigStringRepository, objectLogItemExternalService, loggerProvider.CreateLoggerDefault())
        {
        }

        protected override sealed IUnitOfWorkAsync CreateUnitOfWork()
        {
            return new EcommerceUnitOfWork();
        }
    }
}