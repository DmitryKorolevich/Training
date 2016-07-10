using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Templates;
using Templates.Exceptions;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.UnitOfWork;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
	public class ContentEditService : IContentEditService
    {
        private readonly IRepositoryAsync<MasterContentItem> masterContentItemRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
	    private readonly ILogger _logger;

	    public ContentEditService(IRepositoryAsync<MasterContentItem> masterContentItemRepository,
	        IRepositoryAsync<ContentItem> contentItemRepository, 
	        ILoggerProviderExtended loggerProvider)
	    {
	        this.masterContentItemRepository = masterContentItemRepository;
	        this.contentItemRepository = contentItemRepository;
	        _logger = loggerProvider.CreateLogger<ContentEditService>();
	    }

	    #region Public

	    public async Task<ContentItem> UpdateContentItemAsync(ContentItem itemToUpdate)
	    {
	        await contentItemRepository.UpdateAsync(itemToUpdate);
	        return itemToUpdate;
	    }

	    public async Task<ContentItem> GetContentItemAsync(int id)
	    {
	        return (await contentItemRepository.Query(c => c.Id == id).SelectFirstOrDefaultAsync(false));
        }

	    public virtual async Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem itemToUpdate)
	    {
            await masterContentItemRepository.UpdateAsync(itemToUpdate);
            return itemToUpdate;
        }

	    public virtual async Task<MasterContentItem> GetMasterContentItemAsync(int id)
	    {
	        return (await masterContentItemRepository.Query(m => m.Id == id).SelectFirstOrDefaultAsync(false));
	    }

	    #endregion
    }
}
