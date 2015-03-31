using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Contracts
{
	public interface IContentService
	{
        Task<ExecutedContentItem> GetCategoryContentAsync(ContentType type,string categoryUrl = null);

        Task<ExecutedContentItem> GetContentItemContentAsync(ContentType type, string contentDataItemUrl);
    }
}
