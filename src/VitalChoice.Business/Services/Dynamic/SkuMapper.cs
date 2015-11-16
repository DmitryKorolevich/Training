using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Business.Services.Dynamic
{
    public class SkuMapper : DynamicMapper<SkuDynamic, Sku, ProductOptionType, SkuOptionValue>
    {
        public SkuMapper(ITypeConverter converter,
            IModelConverterService converterService,
            IEcommerceRepositoryAsync<ProductOptionType> productRepositoryAsync)
            : base(converter, converterService, productRepositoryAsync)
        {

        }

        protected override Expression<Func<SkuOptionValue, int>> ObjectIdReferenceSelector
        {
            get { return c => c.IdSku; }
        }

        //public override IQueryOptionType<ProductOptionType> GetOptionTypeQuery()
        //{
        //    throw new ApiException("Cannot get sku option types as they inherited from product, please provide types with entity or as parameter");
        //}

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
                dynamic.IdProduct = entity.IdProduct;
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
            });
            return Task.Delay(0);
        }
    }
}
