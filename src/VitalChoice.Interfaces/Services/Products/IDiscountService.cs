using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IDiscountService : IDynamicObjectServiceAsync<DiscountDynamic, Discount>
	{
        Task<PagedList<DiscountDynamic>> GetDiscountsAsync(DiscountFilter filter);
    }
}
