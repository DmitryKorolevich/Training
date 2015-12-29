using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class AffiliateMapper : DynamicMapper<AffiliateDynamic, Affiliate, AffiliateOptionType, AffiliateOptionValue>
    {
        public AffiliateMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<AffiliateOptionType> optionTypesRepositoryAsync)
            : base(converter, converterService, optionTypesRepositoryAsync)
        {
        }

        protected override Expression<Func<AffiliateOptionValue, int>> ObjectIdReferenceSelector
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
                dynamic.Email = entity.Email;
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
                entity.Email = dynamic.Email;
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

                if (entity.User == null)
                {
                    entity.User = new User();
                }
                entity.User.Id = dynamic.Id;
                entity.Name = dynamic.Name;
                entity.Email = dynamic.Email;
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