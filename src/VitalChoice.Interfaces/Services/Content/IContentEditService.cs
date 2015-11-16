using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IContentEditService
    {
        Task<ContentViewModel> GetCategoryContentAsync(ContentType type, Dictionary<string, object> parameters, string categoryUrl = null);

        Task<ContentViewModel> GetContentItemContentAsync(ContentType type, Dictionary<string, object> parameters, string contentDataItemUrl);

	    Task<ContentItem> UpdateContentItemAsync(ContentItem itemToUpdate);

	    Task<ContentItem> GetContentItemAsync(int id);

        Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem itemToUpdate);

        Task<MasterContentItem> GetMasterContentItemAsync(int id);
    }
}
