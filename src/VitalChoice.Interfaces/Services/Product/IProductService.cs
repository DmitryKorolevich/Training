using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Product;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Product;

namespace VitalChoice.Interfaces.Services.Product
{
	public interface IProductService
    {
        Task<ICollection<ProductOptionType>> GetProductOptionTypesAsync(ICollection<string> names);

        Task<Dictionary<int, Dictionary<string, string>>> GetProductEditDefaultSettingsAsync();

        Task<ICollection<ProductOptionType>> GetProductLookupsAsync();

        Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter);

        Task<object> GetProductAsync(int id, bool withDefaults = false);

        Task<object> UpdateProductAsync(object modelO);

        Task<bool> DeleteProductAsync(int id);
    }
}
