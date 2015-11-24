using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Promotion;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IPromotionService : IDynamicServiceAsync<PromotionDynamic, Promotion>
	{
        Task<PagedList<PromotionDynamic>> GetPromotionsAsync(PromotionFilter filter);

	    Task<List<PromotionDynamic>> GetActivePromotions(CustomerType customerType);
	}
}
