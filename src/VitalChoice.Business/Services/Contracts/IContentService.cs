using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Contracts
{
	public interface IContentService
	{
        Task<ExecutedContentItem> GetCategoryContentAsync(ContentType type, Dictionary<string, object> parameters, string categoryUrl = null);

        Task<ExecutedContentItem> GetContentItemContentAsync(ContentType type, Dictionary<string, object> parameters, string contentDataItemUrl);

	    Task<bool> UpdateContentItemAsync(ContentItem itemToUpdate);

	    Task<ContentItem> GetContentItemAsync(int id);

        Task<bool> UpdateMasterContentItemAsync(MasterContentItem itemToUpdate);

        Task<MasterContentItem> GetMasterContentItemAsync(int id);
    }
}
