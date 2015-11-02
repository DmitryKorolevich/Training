using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IProductViewService
    {
        Task<ContentViewModel> GetProductCategoryContentAsync(IList<CustomerTypeCode> customerTypes, Dictionary<string, object> parameters);
    }
}
