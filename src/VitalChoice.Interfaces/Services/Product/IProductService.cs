using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Product;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Product;

namespace VitalChoice.Interfaces.Services.Product
{
	public interface IProductService
    {
        Task<ICollection<ProductOptionType>> GetProductLookupsAsync();

        Task<PagedList<VProductSku>> GetProductsAsync(VProductSkuFilter filter);
    }
}
