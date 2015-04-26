using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Contracts.Product
{
	public interface IProductViewService
    {
        Task<ExecutedContentItem> GetProductCategoryContentAsync(Dictionary<string, object> parameters, string categoryUrl = null);
    }
}
