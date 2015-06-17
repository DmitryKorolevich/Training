using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Infrastructure.Services;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
    public class GeneralContentService : IGeneralContentService
    {
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<ContentProcessor> contentProcessorRepository;
        private readonly ILogger _logger;

        public GeneralContentService(IRepositoryAsync<ContentProcessor> contentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository, ILoggerProviderExtended loggerProvider)
        {
            this.contentTypeRepository = contentTypeRepository;
            this.contentProcessorRepository = contentProcessorRepository;
            _logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<List<ContentTypeEntity>> GetContentTypesAsync()
        {
            return await contentTypeRepository.Query().SelectAsync(false);
        }

        public async Task<List<ContentProcessor>> GetContentProcessorsAsync()
        {
            return await contentProcessorRepository.Query().SelectAsync(false);
        }
    }
}
