using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VC.Admin.Models.Products
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