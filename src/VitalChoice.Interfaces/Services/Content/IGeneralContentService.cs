using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IGeneralContentService
    {
        Task<List<ContentTypeEntity>> GetContentTypesAsync();
        Task<List<ContentProcessor>> GetContentProcessorsAsync();
    }
}
