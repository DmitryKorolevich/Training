using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.DynamicData.Entities.Transfer
{
    public class ProductContentTransferEntity
    {
        public ProductContent ProductContent { get; set; }

        public ProductDynamic ProductDynamic { get; set; }
    }
}
