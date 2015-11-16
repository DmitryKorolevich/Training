using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Admin.Models.Product
{
    public class SkuWithStatisticListItemModel : SkuListItemModel
    {
        public int Ordered { get; set; }

        public SkuWithStatisticListItemModel(VSku item, int ordered) : base(item)
        {
            if(item!=null)
            {
                Ordered = ordered;
            }
        }
    }
}