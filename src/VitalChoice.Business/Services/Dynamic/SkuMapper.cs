using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class SkuMapper : DynamicObjectMapper<SkuDynamic, Sku, ProductOptionType, ProductOptionValue>
    {
        public SkuMapper(IIndex<Type, IDynamicToModelMapper> mappers,
            IIndex<TypePair, IModelToDynamicConverter> container,
            IEcommerceRepositoryAsync<ProductOptionType> productRepositoryAsync)
            : base(mappers, container, productRepositoryAsync)
        {

        }

        public override Expression<Func<ProductOptionValue, int?>> ObjectIdSelector
        {
            get { return c => c.IdSku; }
        }

        public override IQueryOptionType<ProductOptionType> GetOptionTypeQuery()
        {
            throw new ApiException("Cannot get sku option types as they inherited from product, please provide types with entity or as parameter");
        }

        protected override Task FromEntityRangeInternalAsync(ICollection<DynamicEntityPair<SkuDynamic, Sku>> items, bool withDefaults = false)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                dynamic.Code = entity.Code;
                dynamic.Hidden = entity.Hidden;
                dynamic.Price = entity.Price;
                dynamic.WholesalePrice = entity.WholesalePrice;
                dynamic.Order = entity.Order;
            });
            return Task.Delay(0);
        }

        protected override Task UpdateEntityRangeInternalAsync(ICollection<DynamicEntityPair<SkuDynamic, Sku>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.Code = dynamic.Code;
                entity.Hidden = dynamic.Hidden;
                entity.Price = dynamic.Price;
                entity.WholesalePrice = dynamic.WholesalePrice;
                entity.Order = dynamic.Order;
            });
            return Task.Delay(0);
        }

        protected override Task ToEntityRangeInternalAsync(ICollection<DynamicEntityPair<SkuDynamic, Sku>> items)
        {
            items.ForEach(pair =>
            {
                var entity = pair.Entity;
                var dynamic = pair.Dynamic;

                entity.Code = dynamic.Code;
                entity.Hidden = dynamic.Hidden;
                entity.Price = dynamic.Price;
                entity.WholesalePrice = dynamic.WholesalePrice;
                entity.Order = dynamic.Order;

                //Set key on options
                foreach (var value in entity.OptionValues)
                {
                    value.IdSku = dynamic.Id;
                }
            });
            return Task.Delay(0);
        }
    }
}
