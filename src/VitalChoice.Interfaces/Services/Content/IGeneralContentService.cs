using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IGeneralContentService
    {
        Task<List<ContentTypeEntity>> GetContentTypesAsync();
        Task<List<ContentProcessorEntity>> GetContentProcessorsAsync();
    }
}
