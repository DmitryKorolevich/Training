using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface ICategoryViewService
    {
        Task<ContentViewModel> GetProductCategoryContentAsync(IList<CustomerTypeCode> customerTypes, Dictionary<string, object> parameters);
    }
}
