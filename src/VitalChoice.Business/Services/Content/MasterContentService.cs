using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Content.Emails;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
    public class MasterContentService : IMasterContentService
    {
        private readonly IRepositoryAsync<MasterContentItem> _masterContentItemRepository;
        private readonly IRepositoryAsync<MasterContentItemToContentProcessor> _masterContentItemToProcessorsRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> _contentTypeRepository;
        private readonly IRepositoryAsync<ContentCategory> _contentCategoryRepository;
        private readonly IRepositoryAsync<Recipe> _recipeRepository;
        private readonly IRepositoryAsync<FAQ> _faqRepository;
        private readonly IRepositoryAsync<Article> _articleRepository;
        private readonly IRepositoryAsync<ContentPage> _contentPageRepository;
        private readonly IRepositoryAsync<ProductContent> _productContentRepository;
        private readonly IRepositoryAsync<EmailTemplate> _emailTemplateRepository;
        private readonly IObjectLogItemExternalService _objectLogItemExternalService;
        private readonly ILogger _logger;

        public MasterContentService(IRepositoryAsync<MasterContentItem> masterContentItemRepository,
            IRepositoryAsync<MasterContentItemToContentProcessor> masterContentItemToProcessorsRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IRepositoryAsync<ContentCategory> contentCategoryRepository, IRepositoryAsync<Recipe> recipeRepository,
            IRepositoryAsync<FAQ> faqRepository, IRepositoryAsync<Article> articleRepository,
            IRepositoryAsync<ContentPage> contentPageRepository, ILoggerFactory loggerProvider,
            IRepositoryAsync<ProductContent> productContentRepository, IRepositoryAsync<EmailTemplate> emailTemplateRepository,
            IObjectLogItemExternalService objectLogItemExternalService)
        {
            this._masterContentItemRepository = masterContentItemRepository;
            this._masterContentItemToProcessorsRepository = masterContentItemToProcessorsRepository;
            this._contentTypeRepository = contentTypeRepository;
            this._contentCategoryRepository = contentCategoryRepository;
            this._recipeRepository = recipeRepository;
            this._faqRepository = faqRepository;
            this._articleRepository = articleRepository;
            this._contentPageRepository = contentPageRepository;
            _productContentRepository = productContentRepository;
            _emailTemplateRepository = emailTemplateRepository;
            _objectLogItemExternalService = objectLogItemExternalService;
            _logger = loggerProvider.CreateLogger<MasterContentService>();
        }

        public async Task<List<ContentTypeEntity>> GetContentTypesAsync()
        {
            return await _contentTypeRepository.Query().SelectAsync(false);
        }

        public async Task<List<MasterContentItem>> GetMasterContentItemsAsync(MasterContentItemListFilter filter)
        {
            var query = new MasterContentItemQuery();
            query = query.WithType(filter.Type).NotDeleted().WithStatus(filter.Status);

            Func<IQueryable<MasterContentItem>, IOrderedQueryable<MasterContentItem>> sortable = x => x.OrderBy(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case MasterContentSortPath.Name:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
                case MasterContentSortPath.Type:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Type != null ? y.Type.Name : null)
                                : x.OrderByDescending(y => y.Type != null ? y.Type.Name : null);
                    break;
                case MasterContentSortPath.Updated:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Updated)
                                : x.OrderByDescending(y => y.Updated);
                    break;
            }

            return
                await
                    _masterContentItemRepository.Query(query)
                        .Include(p => p.Type)
                        .Include(p => p.User)
                        .ThenInclude(p => p.Profile)
                        .OrderBy(sortable)
                        .SelectAsync(false);
        }

        public async Task<MasterContentItem> GetMasterContentItemAsync(int id)
        {
            var query = new MasterContentItemQuery();
            query = query.WithId(id).NotDeleted();
            var toReturn =
                (await _masterContentItemRepository.Query(query).Include(p => p.MasterContentItemToContentProcessors).Include(p => p.Type).
                    Include(p => p.User).ThenInclude(p => p.Profile).SelectFirstOrDefaultAsync(false));
            return toReturn;
        }

        public async Task<MasterContentItem> UpdateMasterContentItemAsync(MasterContentItem model)
        {
            MasterContentItem dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new MasterContentItem
                {
                    TypeId = model.Type.Id,
                    StatusCode = RecordStatusCode.Active,
                    MasterContentItemToContentProcessors = new List<MasterContentItemToContentProcessor>(),
                    Created = DateTime.Now
                };
            }
            else
            {
                dbItem =
                    await
                        _masterContentItemRepository.Query(p => p.Id == model.Id)
                            .Include(p => p.MasterContentItemToContentProcessors)
                            .Include(p => p.Type)
                            .SelectFirstOrDefaultAsync(true);
                if (dbItem != null)
                {
                    await _masterContentItemToProcessorsRepository.DeleteAllAsync(dbItem.MasterContentItemToContentProcessors);
                }
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                var idDbItem = dbItem.Id;

                var nameDublicatesExist =
                    await
                        _masterContentItemRepository.Query(
                            p => p.Name == model.Name && p.Id != idDbItem && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();

                if (nameDublicatesExist)
                {
                    throw new AppValidationException("Name", "Master content item with the same name is already exist.");
                }

                dbItem.Name = model.Name;
                dbItem.Template = model.Template;
                dbItem.IdEditedBy = model.IdEditedBy;
                dbItem.Updated = DateTime.Now;

                if (model.Id == 0)
                {
                    await _masterContentItemRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    await _masterContentItemRepository.UpdateAsync(dbItem);
                }

                foreach (var processor in model.MasterContentItemToContentProcessors)
                {
                    processor.MasterContentItemId = dbItem.Id;
                }
                await _masterContentItemToProcessorsRepository.InsertRangeAsync(model.MasterContentItemToContentProcessors);

                if (model.Type.DefaultMasterContentItemId.HasValue)
                {
                    var contentType = await _contentTypeRepository.Query(p => p.Id == dbItem.TypeId).SelectFirstOrDefaultAsync(true);
                    if (contentType != null && contentType.DefaultMasterContentItemId != model.Type.DefaultMasterContentItemId)
                    {
                        contentType.DefaultMasterContentItemId = dbItem.Id;
                        await _contentTypeRepository.UpdateAsync(contentType);
                    }
                }

                await _objectLogItemExternalService.LogItem(dbItem);
            }

            return dbItem;
        }

        public async Task<bool> DeleteMasterContentItemAsync(int id)
        {
            var dbItem = await _masterContentItemRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(true);
            if (dbItem != null)
            {
                var contentType = await _contentTypeRepository.Query(t => t.Id == dbItem.TypeId).SelectFirstOrDefaultAsync(false);
                if (contentType.DefaultMasterContentItemId == dbItem.Id)
                {
                    throw new AppValidationException(
                        "The given master templates is set as default. Please set another master as default to delete this one.");
                }
                switch ((ContentType) dbItem.TypeId)
                {
                    case ContentType.RecipeCategory:
                    case ContentType.ArticleCategory:
                    case ContentType.ContentPageCategory:
                    case ContentType.ProductCategory:
                    case ContentType.FaqCategory:
                        if (await _contentCategoryRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync())
                        {
                            throw new AppValidationException(
                                "The given master templates is used. Reassign master on related Content Categories first.");
                        }
                        break;
                    case ContentType.Recipe:
                        if (await _recipeRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync())
                        {
                            throw new AppValidationException(
                                "The given master templates is used. Reassign master on related Recipes first.");
                        }
                        break;
                    case ContentType.Article:
                        if (await _articleRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync())
                        {
                            throw new AppValidationException(
                                "The given master templates is used. Reassign master on related Articles first.");
                        }
                        break;
                    case ContentType.Faq:
                        if (await _faqRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync())
                        {
                            throw new AppValidationException(
                                "The given master templates is used. Reassign master on related FAQs first.");
                        }
                        break;
                    case ContentType.ContentPage:
                        if (await _contentPageRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync())
                        {
                            throw new AppValidationException(
                                "The given master templates is used. Reassign master on related Content Pages first.");
                        }
                        break;
                    case ContentType.Product:
                        if (await _productContentRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync())
                        {
                            throw new AppValidationException(
                                "The given master templates is used. Reassign master on related Products first.");
                        }
                        break;
                    case ContentType.Email:
                        if (await _emailTemplateRepository.Query(p => p.MasterContentItemId == id).SelectAnyAsync())
                        {
                            throw new AppValidationException(
                                "The given master templates is used. Reassign master on related Email Templates first.");
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await _masterContentItemRepository.UpdateAsync(dbItem);
                return true;
            }
            return false;
        }
    }
}