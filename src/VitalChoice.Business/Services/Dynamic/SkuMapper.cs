using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Business.Services.Dynamic
{
    public class SkuMapper : DynamicObjectMapper<SkuDynamic, Sku, ProductOptionType, ProductOptionValue>
    {
        public SkuMapper(IIndex<Type, IDynamicToModelMapper> mappers, IIndex<Type, IModelToDynamicConverter> container, IEcommerceRepositoryAsync<ProductOptionType> productRepositoryAsync)
            : base(mappers, container, productRepositoryAsync)
        {

        }

        protected override void ToEntityInternal(SkuDynamic dynamic, Sku entity)
        {
            ToEntityStd(dynamic, entity);
        }

        public override IQueryObject<ProductOptionType> GetOptionTypeQuery(int? idType)
        {
            return new ProductOptionTypeQuery().WithType((ProductType?) idType);
        }

        protected override void FromEntityInternal(SkuDynamic dynamic, Sku entity, bool withDefaults = false)
        {
            dynamic.Code = entity.Code;
            dynamic.Hidden = entity.Hidden;
            dynamic.Price = entity.Price;
            dynamic.WholesalePrice = entity.WholesalePrice;
            dynamic.Order = entity.Order;
        }

        protected override void UpdateEntityInternal(SkuDynamic dynamic, Sku entity)
        {
            ToEntityStd(dynamic, entity);

            //Set key on options
            foreach (var value in entity.OptionValues)
            {
                value.IdSku = dynamic.Id;
            }
        }

        private static void ToEntityStd(SkuDynamic dynamic, Sku entity)
        {
            entity.Code = dynamic.Code;
            entity.Hidden = dynamic.Hidden;
            entity.Price = dynamic.Price;
            entity.WholesalePrice = dynamic.WholesalePrice;
            entity.Order = dynamic.Order;
        }
    }
}
