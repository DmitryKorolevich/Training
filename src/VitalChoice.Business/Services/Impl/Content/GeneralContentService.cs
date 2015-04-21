using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Impl.Content
{
    public class GeneralContentService : IGeneralContentService
    {
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<ContentProcessor> contentProcessorRepository;
        private readonly ILogger _logger;

        public GeneralContentService(IRepositoryAsync<ContentProcessor> contentProcessorRepository, IRepositoryAsync<ContentTypeEntity> contentTypeRepository)
        {
            this.contentTypeRepository = contentTypeRepository;
            this.contentProcessorRepository = contentProcessorRepository;
            _logger = LoggerService.GetDefault();
        }

        public async Task<IEnumerable<ContentTypeEntity>> GetContentTypesAsync()
        {
            var toReturn = (await contentTypeRepository.Query().SelectAsync(false)).ToList();
            return toReturn;
        }

        public async Task<IEnumerable<ContentProcessor>> GetContentProcessorsAsync()
        {
            var toReturn = (await contentProcessorRepository.Query().SelectAsync(false)).ToList();
            return toReturn;
        }
    }
}
