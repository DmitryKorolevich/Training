using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.Promotion;
using VitalChoice.Domain.Entities.eCommerce.Promotions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IPromotionService : IDynamicServiceAsync<PromotionDynamic, Promotion>
	{
        Task<PagedList<PromotionDynamic>> GetPromotionsAsync(PromotionFilter filter);
    }
}
