using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Contracts.Content
{
	public interface IContentViewService
    {
        Task<ExecutedContentItem> GetCategoryContentAsync(ContentType type, Dictionary<string, object> parameters, string categoryUrl = null);

        Task<ExecutedContentItem> GetContentItemContentAsync(ContentType type, Dictionary<string, object> parameters, string contentDataItemUrl);

	    Task<ContentItem> UpdateContentItemAsync(ContentItem itemToUpdate);

	    Task<ContentItem> GetContentItemAsync(int id);

        Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem itemToUpdate);

        Task<MasterContentItem> GetMasterContentItemAsync(int id);
    }
}
