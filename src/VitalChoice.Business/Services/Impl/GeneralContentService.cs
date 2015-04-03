using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Business.Queries.Comment;
using VitalChoice.Business.Queries.User;
using VitalChoice.Business.Services.Contracts;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Infrastructure.UnitOfWork;
using System.Threading;
using Microsoft.Framework.ConfigurationModel;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;
using Microsoft.Framework.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Domain.Exceptions;

namespace VitalChoice.Business.Services.Impl
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
