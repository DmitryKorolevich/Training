using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class SkuDynamic : DynamicObject<Sku, ProductOptionValue, ProductOptionType>
    {
        public SkuDynamic()
        {
            
        }

        public SkuDynamic(Sku entity, bool withDefaults = false) : base(entity, withDefaults)
        {
            
        }

        public string Code { get; set; }

        public bool Hidden { get; set; }

        public decimal Price { get; set; }

        public decimal WholesalePrice { get; set; }

        protected override void FromEntity(Sku entity)
        {
            BaseConvert(entity);
        }

        protected override void FromEntityWithDefaults(Sku entity)
        {
            BaseConvert(entity);
        }

        protected override void FillEntity(Sku entity)
        {
            entity.Code = Code;
            entity.Hidden = Hidden;
            entity.Price = Price;
            entity.WholesalePrice = WholesalePrice;
        }

        private void BaseConvert(Sku entity)
        {
            Code = entity.Code;
            Hidden = entity.Hidden;
            Price = entity.Price;
            WholesalePrice = entity.WholesalePrice;
        }
    }
}