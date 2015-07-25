using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.OptionsModel;
using Microsoft.Framework.Runtime;
using Microsoft.Framework.Runtime.Infrastructure;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Infrastructure.Context;

namespace VitalChoice.Tests.Mapping
{
    public class DbMappingTester<TDynamic, TEntity, TOptionType, TOptionValue>
        where TDynamic: MappedObject
        where TEntity: DynamicDataEntity<TOptionValue, TOptionType>, new() 
        where TOptionValue : OptionValue<TOptionType>, new() 
        where TOptionType : OptionType, new()
    {
        private readonly IDynamicObjectServiceAsync<TDynamic, TEntity> _service;
        private readonly IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> _mapper;
        private readonly IReadRepositoryAsync<TOptionType> _optionTypesRepositoryAsync;

        public DbMappingTester(IDynamicObjectServiceAsync<TDynamic, TEntity> service, IDynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> mapper)
        {
            _service = service;
            _mapper = mapper;
            IServiceCollection serviceCollection =
                (IServiceCollection) CallContextServiceLocator.Locator.ServiceProvider.GetService(typeof (IServiceCollection));
            _optionTypesRepositoryAsync =
                new EcommerceRepositoryAsync<TOptionType>(
                    new EcommerceContext(
                        (IOptions<AppOptions>)
                            serviceCollection.Single(
                                descriptor => descriptor.ServiceType == typeof (IOptions<AppOptions>))
                                .ImplementationInstance));
        }
    }
}
