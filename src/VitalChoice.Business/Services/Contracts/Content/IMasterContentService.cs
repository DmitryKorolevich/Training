using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Contracts.Content
{
	public interface IMasterContentService
    {
        Task<IEnumerable<ContentTypeEntity>> GetContentTypesAsync();
        Task<IEnumerable<MasterContentItem>> GetMasterContentItemsAsync(ContentType? type=null, RecordStatusCode? status=null);
        Task<MasterContentItem> GetMasterContentItemAsync(int id);
        Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem model);
        Task<bool> DeleteMasterContentItemAsync(int id);
    }
}
