using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services.InventorySkus;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class OrderReviewRuleMapper : DynamicMapper<OrderReviewRuleDynamic, OrderReviewRule,
        OrderReviewRuleOptionType, OrderReviewRuleOptionValue>
    {
        public OrderReviewRuleMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<OrderReviewRuleOptionType> repositoryAsync)
            : base(converter, converterService, repositoryAsync)
        {
        }

        public override Expression<Func<OrderReviewRuleOptionValue, int>> ObjectIdSelector => o => o.IdOrderReviewRule;

        protected override Task FromEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderReviewRuleDynamic, OrderReviewRule>> items,
            bool withDefaults = false)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                dynamic.IdAddedBy = entity.IdAddedBy;
                dynamic.Name = entity.Name;
                dynamic.ApplyType = entity.ApplyType;
            });

            return TaskCache.CompletedTask;
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<OrderReviewRuleDynamic, OrderReviewRule>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdAddedBy = entity.IdEditedBy;
                entity.Name = dynamic.Name;
                entity.ApplyType = dynamic.ApplyType;
            });

            return TaskCache.CompletedTask;
        }

        protected override Task UpdateEntityRangeInternalAsync(
            ICollection<DynamicEntityPair<OrderReviewRuleDynamic, OrderReviewRule>> items)
        {
            items.ForEach(item =>
            {
                var entity = item.Entity;
                var dynamic = item.Dynamic;

                entity.IdAddedBy = entity.IdAddedBy;
                entity.Name = dynamic.Name;
                entity.ApplyType = dynamic.ApplyType;
            });

            return TaskCache.CompletedTask;
        }
    }
}