using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Templates;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;

namespace VitalChoice.Business.Services.Impl.Content
{
    public class ContentPageService : IContentPageService
    {
        private readonly IRepositoryAsync<ContentPage> contentPageRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<ContentPageToContentCategory> contentPageToContentCategoryRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public ContentPageService(IRepositoryAsync<ContentPage> contentPageRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository, IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<ContentPageToContentCategory> contentPageToContentCategoryRepository, IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository)
        {
            this.contentPageRepository = contentPageRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentPageToContentCategoryRepository = contentPageToContentCategoryRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            logger = LoggerService.GetDefault();
        }
        
        public async Task<PagedList<ContentPage>> GetContentPagesAsync(ContentPageListFilter filter)
        {
            ContentPageQuery query = new ContentPageQuery();
            List<int> ids = null;
            if(filter.CategoryId.HasValue)
            {
                if (filter.CategoryId.Value != -1)
                {
                    ids = (await contentPageToContentCategoryRepository.Query(p => p.ContentCategoryId == filter.CategoryId.Value).SelectAsync(false)).Select(p => p.ContentPageId).ToList();
                    query = query.WithIds(ids);
                }
                else
                {
                    ids = (await contentPageToContentCategoryRepository.Query().SelectAsync(false)).Select(p => p.ContentPageId).Distinct().ToList();
                    query = query.NotWithIds(ids);
                }
            }
            query=query.WithName(filter.Name).NotDeleted();
            var toReturn = await contentPageRepository.Query(query).Include(p=>p.ContentItem).Include(p => p.ContentPagesToContentCategories).ThenInclude(p => p.ContentCategory).OrderBy(x => x.OrderBy(pp => pp.Name)).
                SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

        public async Task<ContentPage> GetContentPageAsync(int id)
        {
            ContentPageQuery query = new ContentPageQuery().WithId(id).NotDeleted();
            var toReturn = (await contentPageRepository.Query(query).Include(p=>p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
                Include(p=>p.ContentPagesToContentCategories).
                SelectAsync(false)).FirstOrDefault();
            return toReturn;
        }

        public async Task<ContentPage> UpdateContentPageAsync(ContentPage model)
        {
            ContentPage dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new ContentPage();
                dbItem.StatusCode = RecordStatusCode.Active;
                dbItem.ContentItem = new ContentItem();
                dbItem.ContentItem.Created = DateTime.Now;
                dbItem.ContentItem.ContentItemToContentProcessors = new List<ContentItemToContentProcessor>();

                if(model.MasterContentItemId==0)
                { 
                    //set predefined master
                    var contentType = (await contentTypeRepository.Query(p => p.Id == (int)ContentType.ContentPage).SelectAsync()).FirstOrDefault();
                    if (contentType == null || !contentType.DefaultMasterContentItemId.HasValue)
                    {
                        throw new Exception("The default master template isn't confugurated. Please contact support.");
                    }
                    model.MasterContentItemId = contentType.DefaultMasterContentItemId.Value;
                }
            }
            else
            {
                dbItem = (await contentPageRepository.Query(p => p.Id == model.Id).Include(p => p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
                    SelectAsync()).FirstOrDefault();
                if (dbItem != null)
                {
                    foreach (var proccesorRef in dbItem.ContentItem.ContentItemToContentProcessors)
                    {
                        await contentItemToContentProcessorRepository.DeleteAsync(proccesorRef.Id);
                    }
                }
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                var urlDublicatesExist = await contentPageRepository.Query(p => p.Url == model.Url && p.Id != dbItem.Id
                    && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                if (urlDublicatesExist)
                {
                    throw new AppValidationException("Url","Content page with the same URL already exists, please use a unique URL.");
                }

                dbItem.Name = model.Name;
                dbItem.Url = model.Url;
                dbItem.FileUrl = model.FileUrl;
                if (model.StatusCode != RecordStatusCode.Deleted)
                {
                    dbItem.StatusCode = model.StatusCode;
                }
                dbItem.Assigned = model.Assigned;
                dbItem.MasterContentItemId = model.MasterContentItemId;
                dbItem.ContentItem.Updated = DateTime.Now;
                dbItem.ContentItem.Template = model.ContentItem.Template;
                dbItem.ContentItem.Description = model.ContentItem.Description;
                dbItem.ContentItem.Title = model.ContentItem.Title;
                dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
                dbItem.ContentItem.ContentItemToContentProcessors = model.ContentItem.ContentItemToContentProcessors;

                if (model.Id == 0)
                {
                    dbItem = await contentPageRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    dbItem = await contentPageRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> AttachContentPageToCategoriesAsync(int id, IEnumerable<int> categoryIds)
        {
            bool toReturn = false;
            var dbItem = (await contentPageRepository.Query(p => p.Id == id).Include(p=>p.ContentPagesToContentCategories).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                var categories = (await contentCategoryRepository.Query(p => categoryIds.Contains(p.Id) && p.Type == ContentType.ContentPageCategory && p.StatusCode != RecordStatusCode.Deleted).
                                 SelectAsync(false)).ToList();

                List<int> forDelete = new List<int>();
                foreach (var contentPageToContentCategory in dbItem.ContentPagesToContentCategories)
                {
                    bool delete = true;
                    foreach (var category in categories)
                    {
                        if(contentPageToContentCategory.ContentCategoryId==category.Id)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if(delete)
                    {
                        forDelete.Add(contentPageToContentCategory.Id);
                    }
                }

                List<int> forAdd = new List<int>();
                foreach (var category in categories)
                {
                    bool add = true;
                    foreach (var contentPageToContentCategory in dbItem.ContentPagesToContentCategories)
                    {
                        if (contentPageToContentCategory.ContentCategoryId == category.Id)
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        forAdd.Add(category.Id);
                    }
                }

                foreach (var forDeleteId in forDelete)
                {
                    await contentPageToContentCategoryRepository.DeleteAsync(forDeleteId);
                }
                await contentPageToContentCategoryRepository.InsertRangeAsync(forAdd.Select(p => new ContentPageToContentCategory()
                {
                    ContentCategoryId = p,
                    ContentPageId = dbItem.Id
                }).ToList());

                toReturn = true;
            }

            return toReturn;
        }

        public async Task<bool> DeleteContentPageAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await contentPageRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await contentPageRepository.UpdateAsync(dbItem);

                try
                {
                    templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId);
                }
                catch(Exception e)
                {
                    logger.LogError(e.ToString());
                }
                toReturn = true;
            }
            return toReturn;
        }
    }
}