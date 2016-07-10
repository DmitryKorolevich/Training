using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Settings;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class SettingMapper : DynamicMapper<SettingDynamic, Setting, SettingOptionType, SettingOptionValue>
    {
        public SettingMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<SettingOptionType> optionTypesRepositoryAsync)
            : base(converter, converterService, optionTypesRepositoryAsync)
        {
        }

        public override Expression<Func<SettingOptionValue, int>> ObjectIdSelector => a => 0;

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<SettingDynamic, Setting>> items, bool withDefaults = false)
        {
            return TaskCache.CompletedTask;
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<SettingDynamic, Setting>> items)
        {
            return TaskCache.CompletedTask;
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<SettingDynamic, Setting>> items)
        {
            return TaskCache.CompletedTask;
        }
    }
}