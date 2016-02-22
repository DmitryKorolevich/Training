using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
    public class ContentPageService : IContentPageService
    {
        private readonly IRepositoryAsync<ContentPage> contentPageRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<ContentPageToContentCategory> contentPageToContentCategoryRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IObjectLogItemExternalService objectLogItemExternalService;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public ContentPageService(IRepositoryAsync<ContentPage> contentPageRepository,
            IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<ContentPageToContentCategory> contentPageToContentCategoryRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerProviderExtended logger, 
            ITtlGlobalCache templatesCache)
        {
            this.contentPageRepository = contentPageRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.contentPageToContentCategoryRepository = contentPageToContentCategoryRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this.objectLogItemExternalService = objectLogItemExternalService;
            this.templatesCache = templatesCache;
            this.logger = logger.CreateLoggerDefault();
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
                    if (ids.Count == 0)
                    {
                        return new PagedList<ContentPage>()
                        {
                            Count = 0,
                            Items = new ContentPage[0],
                        };
                    }
                    query = query.WithIds(ids);
                }
                else
                {
                    ids = (await contentPageToContentCategoryRepository.Query().SelectAsync(false)).Select(p => p.ContentPageId).Distinct().ToList();
                    query = query.NotWithIds(ids);
                }
            }
            query=query.WithName(filter.Name).NotDeleted();

			Func<IQueryable<ContentPage>, IOrderedQueryable<ContentPage>> sortable = x => x.OrderBy(y => y.Name);
			var sortOrder = filter.Sorting.SortOrder;
			switch (filter.Sorting.Path)
			{
				case ContentPageSortPath.Title:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.Name)
								: x.OrderByDescending(y => y.Name);
					break;
				case ContentPageSortPath.Url:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.Url)
								: x.OrderByDescending(y => y.Url);
					break;
				case ContentPageSortPath.Status:
					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.StatusCode)
								: x.OrderByDescending(y => y.StatusCode);
					break;
				case ContentPageSortPath.Updated:
					//contentPageRepository.EarlyRead = true; //added temporarly till ef 7 becomes stable

					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.ContentItem.Updated)
								: x.OrderByDescending(y => y.ContentItem.Updated);
					break;
			}

			var toReturn = await contentPageRepository.Query(query).Include(p=>p.ContentItem).Include(p => p.ContentPagesToContentCategories).ThenInclude(p => p.ContentCategory).OrderBy(sortable).
                Include(p => p.User).ThenInclude(p => p.Profile).
                SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

        public async Task<ContentPage> GetContentPageAsync(int id)
        {
            ContentPageQuery query = new ContentPageQuery().WithId(id).NotDeleted();
            var toReturn = (await contentPageRepository.Query(query).Include(p=>p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
                Include(p=>p.ContentPagesToContentCategories).Include(p => p.User).ThenInclude(p => p.Profile).
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
                dbItem.UserId = model.UserId;
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

                if (model.Id == 0)
                {
                    await contentPageRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    await contentPageRepository.UpdateAsync(dbItem);
                }

                await objectLogItemExternalService.LogItems(new object[] { dbItem }, true);
            }

            return dbItem;
        }

        public async Task<bool> AttachContentPageToCategoriesAsync(int id, IEnumerable<int> categoryIds)
        {
            bool toReturn = false;
            var dbItem = (await contentPageRepository.Query(p => p.Id == id).Include(p=>p.ContentPagesToContentCategories).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                var categories = await contentCategoryRepository.Query(p => categoryIds.Contains(p.Id) && p.Type == ContentType.ContentPageCategory && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);

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
            var dbItem = (await contentPageRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await contentPageRepository.UpdateAsync(dbItem);

                try
                {
                    await templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId);
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