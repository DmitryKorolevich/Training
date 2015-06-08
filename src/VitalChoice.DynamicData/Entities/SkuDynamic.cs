using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Product;

namespace VitalChoice.DynamicData.Entities
{
    public class SkuDynamic : DynamicObject<Sku, ProductOptionValue, ProductOptionType>
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public bool Hidden { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        public override Sku ToEntity()
        {
            return base.ToEntity();
        }

        public override IDynamicEntity<Sku, ProductOptionValue, ProductOptionType> FromEntity(Sku entity)
        {
            BaseConvert(entity);
            return base.FromEntity(entity);
        }

        public override IDynamicEntity<Sku, ProductOptionValue, ProductOptionType> FromEntityWithDefaults(Sku entity)
        {
            BaseConvert(entity);
            return base.FromEntityWithDefaults(entity);
        }

        private void BaseConvert(Sku entity)
        {
            Id = entity.Id;
            Code = entity.Code;
            StatusCode = entity.StatusCode;
            Hidden = entity.Hidden;
            Price = entity.Price;
            WholesalePrice = entity.WholesalePrice;
        }
    }
}
