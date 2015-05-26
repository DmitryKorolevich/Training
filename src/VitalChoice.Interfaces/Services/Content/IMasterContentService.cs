using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.ContentManagement;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IMasterContentService
    {
        Task<IEnumerable<ContentTypeEntity>> GetContentTypesAsync();
        Task<IEnumerable<MasterContentItem>> GetMasterContentItemsAsync(MasterContentItemListFilter filter);
        Task<MasterContentItem> GetMasterContentItemAsync(int id);
        Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem model);
        Task<bool> DeleteMasterContentItemAsync(int id);
    }
}
