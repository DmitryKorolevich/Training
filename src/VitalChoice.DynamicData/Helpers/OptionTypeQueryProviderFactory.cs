using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.DynamicData.Helpers
{
    public class OptionTypeQueryProviderFactory
    {
        private readonly ILifetimeScope _currentScope;

        public OptionTypeQueryProviderFactory(ILifetimeScope currentScope)
        {
            _currentScope = currentScope;
        }

        public IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue> GetOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue>()
            where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
            where TOptionType : OptionType
            where TOptionValue : OptionValue<TOptionType>
        {
            return _currentScope.Resolve<IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue>>();
        }
    }
}
