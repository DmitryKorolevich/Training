using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Admin.Models.Products
{
    public class SkuWithStatisticListItemModel : SkuListItemModel
    {
        [Map("Order")]
        public int Ordered { get; set; }

        public SkuWithStatisticListItemModel()
        {
            
        }

        public SkuWithStatisticListItemModel(VSku item, int ordered) : base(item)
        {
            if(item!=null)
            {
                Ordered = ordered;
            }
        }
    }
}