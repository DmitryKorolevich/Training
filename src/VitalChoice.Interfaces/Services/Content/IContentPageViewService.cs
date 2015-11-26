using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IContentPageViewService
    {
        Task<ContentViewModel> GetContentAsync(IDictionary<string, object> queryData);
    }
}
