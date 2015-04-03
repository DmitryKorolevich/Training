using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Contracts
{
	public interface IGeneralContentService
    {
        Task<IEnumerable<ContentTypeEntity>> GetContentTypesAsync();
        Task<IEnumerable<ContentProcessor>> GetContentProcessorsAsync();
    }
}
