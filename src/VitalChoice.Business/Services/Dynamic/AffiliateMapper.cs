using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Customer;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;

namespace VitalChoice.Business.Services.Dynamic
{
    public class AffiliateMapper : DynamicObjectMapper<AffiliateDynamic, Affiliate, AffiliateOptionType, AffiliateOptionValue>
    {
        public AffiliateMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<Type, IModelToDynamicConverter> container,
            IEcommerceRepositoryAsync<AffiliateOptionType> optionTypesRepositoryAsync)
            : base(mappers, container, optionTypesRepositoryAsync)
        {
        }

        public override Expression<Func<AffiliateOptionValue, int?>> ObjectIdSelector
        {
            get { return a => a.IdAffiliate; }
        }

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<AffiliateDynamic, Affiliate>> items, bool withDefaults = false)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.Name = entity.Name;
                dynamic.MyAppBalance = entity.MyAppBalance;
                dynamic.CommissionFirst = entity.CommissionFirst;
                dynamic.CommissionAll = entity.CommissionAll;
                dynamic.IdCountry = entity.IdCountry;
                dynamic.IdState = entity.IdState;
                dynamic.County = entity.County;
            });
            return Task.Delay(0);
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<AffiliateDynamic, Affiliate>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.Name = dynamic.Name;
                entity.MyAppBalance = dynamic.MyAppBalance;
                entity.CommissionFirst = dynamic.CommissionFirst;
                entity.CommissionAll = dynamic.CommissionAll;
                entity.IdCountry = dynamic.IdCountry;
                entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
                entity.County = dynamic.County;
            });
            return Task.Delay(0);
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<AffiliateDynamic, Affiliate>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.Name = dynamic.Name;
                entity.MyAppBalance = dynamic.MyAppBalance;
                entity.CommissionFirst = dynamic.CommissionFirst;
                entity.CommissionAll = dynamic.CommissionAll;
                entity.IdCountry = dynamic.IdCountry;
                entity.IdState = dynamic.IdState == 0 ? null : dynamic.IdState;
                entity.County = dynamic.County;
            });
            return Task.Delay(0);
        }
    }
}