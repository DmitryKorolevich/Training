using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;

namespace VitalChoice.Interfaces.Services.Product
{
	public interface IDiscountService
    {
        Task<PagedList<DiscountMapped>> GetDiscountsAsync(DiscountFilter filter);

        Task<DiscountMapped> GetDiscountAsync(int id, bool withDefaults = false);

        Task<DiscountMapped> UpdateDiscountAsync(DiscountMapped model);

        Task<bool> DeleteDiscountAsync(int id);
    }
}
