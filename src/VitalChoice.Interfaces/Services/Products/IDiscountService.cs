using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IDiscountService : IDynamicServiceAsync<DiscountDynamic, Discount>
	{
        Task<PagedList<DiscountDynamic>> GetDiscountsAsync(DiscountFilter filter);
	    Task<DiscountDynamic> GetByCode(string code);
        Task<int> GetDiscountUsed(DiscountDynamic discount, int idCustomer);
	    Task<bool> SetDiscountUsed(DiscountDynamic discount, int idCustomer, bool addUsage = true);
	}
}
