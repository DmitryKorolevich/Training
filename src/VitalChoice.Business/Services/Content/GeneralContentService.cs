using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Data.Repositories;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
    public class GeneralContentService : IGeneralContentService
    {
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<ContentProcessorEntity> contentProcessorRepository;
        private readonly ILogger _logger;

        public GeneralContentService(IRepositoryAsync<ContentProcessorEntity> contentProcessorRepository,
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

        public async Task<List<ContentProcessorEntity>> GetContentProcessorsAsync()
        {
            return await contentProcessorRepository.Query().SelectAsync(false);
        }
    }
}
