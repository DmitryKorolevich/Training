using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Data.Services;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Infrastructure.UOW;

namespace VitalChoice.Business.Services.Content
{
    public class ArticleService : IArticleService
    {
        private readonly IRepositoryAsync<Article> _articleRepository;
        private readonly IRepositoryAsync<ContentCategory> _contentCategoryRepository;
        private readonly IRepositoryAsync<ArticleToContentCategory> _articleToContentCategoryRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> _contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> _contentTypeRepository;
        private readonly IRepositoryAsync<MasterContentItem> _masterContentRepository;
        private readonly IRepositoryAsync<ArticleToProduct> _articleToProductRepository;
        private readonly IRepositoryAsync<ArticleBonusLink> _articleBonusLinkRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly ITtlGlobalCache _templatesCache;
        private readonly DbContextOptions<VitalChoiceContext> _contextOptions;
        private readonly IOptions<AppOptions> _appOptions;
        private readonly IObjectLogItemExternalService _objectLogItemExternalService;
        private readonly ILogger _logger;

        public ArticleService(IRepositoryAsync<Article> articleRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<ArticleToContentCategory> articleToContentCategoryRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IRepositoryAsync<ArticleToProduct> articleToProductRepository,
            IRepositoryAsync<ArticleBonusLink> articleBonusLinkRepository,
            IEcommerceRepositoryAsync<Product> productRepository,
            IObjectLogItemExternalService objectLogItemExternalService,
            ILoggerFactory logger, 
            ITtlGlobalCache templatesCache,
            DbContextOptions<VitalChoiceContext> contextOptions, IOptions<AppOptions> appOptions, IRepositoryAsync<MasterContentItem> masterContentRepository)
        {
            _articleRepository = articleRepository;
            _contentCategoryRepository = contentCategoryRepository;
            _articleToContentCategoryRepository = articleToContentCategoryRepository;
            _contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            _contentTypeRepository = contentTypeRepository;
            _articleToProductRepository = articleToProductRepository;
            _articleBonusLinkRepository = articleBonusLinkRepository;
            _productRepository = productRepository;
            _templatesCache = templatesCache;
            _contextOptions = contextOptions;
            _appOptions = appOptions;
            _masterContentRepository = masterContentRepository;
            _objectLogItemExternalService = objectLogItemExternalService;
            _logger = logger.CreateLogger<ArticleService>();
        }

        public async Task<PagedList<Article>> GetArticlesAsync(ArticleItemListFilter filter)
        {
            ArticleQuery query = new ArticleQuery();
            List<int> ids = null;
            if (filter.CategoryId.HasValue)
            {
                if (filter.CategoryId.Value != -1)
                {
                    //query = query.WithCategoryId(filter.CategoryId.Value);
                    ids =
                        (await
                            _articleToContentCategoryRepository.Query(p => p.ContentCategoryId == filter.CategoryId.Value)
                                .SelectAsync(false)).Select(p => p.ArticleId).ToList();
                    if (ids.Count == 0)
                    {
                        return new PagedList<Article>()
                        {
                            Count = 0,
                            Items = new List<Article>(),
                        };
                    }
                    query = query.WithIds(ids);
                }
                else
                {
                    //query = query.WithoutCategory();
                    ids =
                        (await _articleToContentCategoryRepository.Query().SelectAsync(false)).Select(p => p.ArticleId).Distinct().ToList();
                    query = query.NotWithIds(ids);
                }
            }
            query = query.WithName(filter.Name).NotDeleted().NotWithIds(filter.ExcludeIds);

            Func<IQueryable<Article>, IOrderedQueryable<Article>> sortable = x => x.OrderBy(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case ArticleSortPath.Title:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
                case ArticleSortPath.Url:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.Url)
                                : x.OrderByDescending(y => y.Url);
                    break;
                case ArticleSortPath.Updated:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.ContentItem.Updated)
                                : x.OrderByDescending(y => y.ContentItem.Updated);
                    break;
                case ArticleSortPath.PublishedDate:
                    sortable =
                        (x) =>
                            sortOrder == FilterSortOrder.Asc
                                ? x.OrderBy(y => y.PublishedDate)
                                : x.OrderByDescending(y => y.PublishedDate);
                    break;
            }

            var toReturn =
                await
                    _articleRepository.Query(query)
                        .Include(p => p.ContentItem)
                        .Include(p => p.ArticlesToContentCategories)
                        .ThenInclude(p => p.ContentCategory)
                        .Include(p => p.User)
                        .ThenInclude(p => p.Profile)
                        .OrderBy(sortable)
                        .SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

        public async Task<Article> GetArticleAsync(int id)
        {
            ArticleQuery query = new ArticleQuery().WithId(id).NotDeleted();
            var toReturn = (await _articleRepository.Query(query).Include(p=>p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
                Include(p=>p.ArticlesToContentCategories).Include(p => p.User).ThenInclude(p => p.Profile).
                Include(p => p.ArticlesToProducts).
                SelectAsync(false)).FirstOrDefault();

            var productIds = toReturn?.ArticlesToProducts.Select(p => p.IdProduct).ToList();
            if (productIds?.Count > 0)
            {
                var shortProducts = (await _productRepository.Query(p => productIds.Contains(p.Id) && p.StatusCode != (int)RecordStatusCode.Deleted)
                    .SelectAsync(false)).Select(p => new ShortProductInfo(p)).ToList();
                foreach (var product in toReturn.ArticlesToProducts)
                {
                    foreach (var shortProduct in shortProducts)
                    {
                        if (product.IdProduct == shortProduct.Id)
                        {
                            product.ShortProductInfo = shortProduct;
                            break;
                        }
                    }
                }
            }

            return toReturn;
        }

        public async Task<Article> GetArticleByIdOldAsync(int id)
        {
            ArticleQuery query = new ArticleQuery().WithIdOld(id).NotDeleted();
            var toReturn = (await _articleRepository.Query(query).SelectFirstOrDefaultAsync(false));

            return toReturn;
        }

        public async Task<Article> UpdateArticleAsync(Article model)
        {
            Article dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new Article();
                dbItem.StatusCode = RecordStatusCode.Active;
                dbItem.ContentItem = new ContentItem();
                dbItem.ContentItem.Created = DateTime.Now;
                dbItem.ContentItem.ContentItemToContentProcessors = new List<ContentItemToContentProcessor>();

                //set predefined master
                var contentType =
                    await _contentTypeRepository.Query(p => p.Id == (int) ContentType.Article).SelectFirstOrDefaultAsync(false);
                if (contentType?.DefaultMasterContentItemId == null)
                {
                    throw new Exception("The default master template isn't confugurated. Please contact support.");
                }
                if (
                    await
                        _masterContentRepository.Query(
                            m => m.Id == contentType.DefaultMasterContentItemId.Value && m.StatusCode == RecordStatusCode.Deleted)
                            .SelectAnyAsync())
                {
                    throw new Exception("The default master template has been removed. Please contact support.");
                }
                dbItem.MasterContentItemId = contentType.DefaultMasterContentItemId.Value;
            }
            else
            {
                dbItem =
                    await
                        _articleRepository.Query(p => p.Id == model.Id)
                            .Include(p => p.ContentItem)
                            .ThenInclude(p => p.ContentItemToContentProcessors)
                            .SelectFirstOrDefaultAsync(true);
                if (dbItem != null)
                {
                    foreach (var proccesorRef in dbItem.ContentItem.ContentItemToContentProcessors)
                    {
                        await _contentItemToContentProcessorRepository.DeleteAsync(proccesorRef.Id);
                    }
                }
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                var urlDublicatesExist = await _articleRepository.Query(p => p.Url == model.Url && p.Id != dbItem.Id
                    && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                if (urlDublicatesExist)
                {
                    throw new AppValidationException("Url","Article with the same URL already exists, please use a unique URL.");
                }

                dbItem.Name = model.Name;
                dbItem.Url = model.Url;
                dbItem.FileUrl = model.FileUrl;
                dbItem.UserId = model.UserId;
                dbItem.SubTitle = model.SubTitle;
                dbItem.Author = model.Author;
                dbItem.FileUrl = model.FileUrl;
                dbItem.PublishedDate = model.PublishedDate;
                dbItem.ContentItem.Updated = DateTime.Now;
                dbItem.ContentItem.Template = model.ContentItem.Template;
                dbItem.ContentItem.Description = model.ContentItem.Description;
                dbItem.ContentItem.Title = model.ContentItem.Title;
                dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
                dbItem.ContentItem.ContentItemToContentProcessors = model.ContentItem.ContentItemToContentProcessors;

                ICollection<ArticleToProduct> articlesToProducts = new List<ArticleToProduct>();
                if (model.ArticlesToProducts != null)
                {
                    articlesToProducts = model.ArticlesToProducts.ToList();
                }
                dbItem.ArticlesToProducts = null;

                if (model.Id == 0)
                {
                    await _articleRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    await _articleRepository.UpdateAsync(dbItem);
                }

                foreach (var item in articlesToProducts)
                {
                    item.Id = 0;
                    item.IdArticle = dbItem.Id;
                }
                var dbProducts = await _articleToProductRepository.Query(c => c.IdArticle == dbItem.Id).SelectAsync(true);
                await _articleToProductRepository.DeleteAllAsync(dbProducts);
                await _articleToProductRepository.InsertRangeAsync(articlesToProducts);
                dbItem.ArticlesToProducts = articlesToProducts;

                await _objectLogItemExternalService.LogItem(dbItem);
            }

            return dbItem;
        }

        public async Task<bool> AttachArticleToCategoriesAsync(int id, IEnumerable<int> categoryIds)
        {
            bool toReturn = false;
            using (var uow = new VitalChoiceUnitOfWork(_contextOptions, _appOptions))
            {
                var articleRepository = uow.RepositoryAsync<Article>();
                var articleToContentCategoryRepository = uow.RepositoryAsync<ArticleToContentCategory>();
                var dbItem = (await articleRepository.Query(p => p.Id == id).Include(p => p.ArticlesToContentCategories).SelectFirstOrDefaultAsync(false));
                if (dbItem != null)
                {
                    var categories =
                        await
                            _contentCategoryRepository.Query(
                                p =>
                                    categoryIds.Contains(p.Id) && p.Type == ContentType.ArticleCategory &&
                                    p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);

                    List<int> forDelete = new List<int>();
                    foreach (var articleToContentCategory in dbItem.ArticlesToContentCategories)
                    {
                        bool delete = categories.All(category => articleToContentCategory.ContentCategoryId != category.Id);
                        if (delete)
                        {
                            forDelete.Add(articleToContentCategory.Id);
                        }
                    }

                    List<int> forAdd = new List<int>();
                    foreach (var category in categories)
                    {
                        bool add = dbItem.ArticlesToContentCategories.All(articleToContentCategory => articleToContentCategory.ContentCategoryId != category.Id);
                        if (add)
                        {
                            forAdd.Add(category.Id);
                        }
                    }

                    foreach (var forDeleteId in forDelete)
                    {
                        await articleToContentCategoryRepository.DeleteAsync(forDeleteId);
                    }
                    await articleToContentCategoryRepository.InsertRangeAsync(forAdd.Select(p => new ArticleToContentCategory()
                    {
                        ContentCategoryId = p,
                        ArticleId = dbItem.Id
                    }).ToList());
                    await uow.SaveChangesAsync(CancellationToken.None);

                    toReturn = true;
                }
            }

            return toReturn;
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await _articleRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectFirstOrDefaultAsync(false));
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await _articleRepository.UpdateAsync(dbItem);

                try
                {
                    await _templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId);
                }
                catch(Exception e)
                {
                    _logger.LogError(e.ToString());
                }
                toReturn = true;
            }
            return toReturn;
        }

        public async Task<ICollection<ArticleBonusLink>> GetArticleBonusLinksAsync()
        {
            Func<IQueryable<ArticleBonusLink>, IOrderedQueryable<ArticleBonusLink>> sortable = x => x.OrderByDescending(y => y.StartDate);
            var toReturn = await _articleBonusLinkRepository.Query().OrderBy(sortable).SelectAsync(false);
            return toReturn;
        }

        public async Task<bool> UpdateArticleBonusLinksAsync(ICollection<ArticleBonusLink> items)
        {
            if (items.Count > 10)
            {
                throw new AppValidationException("Can't be more than 10 article bonus links. Please delete not needed links.");
            }

            var dbItems = await _articleBonusLinkRepository.Query().SelectAsync(true);

            var toRemove = dbItems.ExceptKeyedWith(items, p => p.Id).ToArray();

            dbItems.MergeKeyed(items.Where(p => p.Id != 0).ToList(), p => p.Id, (a, b) =>
            {
                a.Url = b.Url;
                a.StartDate = b.StartDate;
            });
            
            await _articleBonusLinkRepository.DeleteAllAsync(toRemove.Select(p=>p.Id).ToList());
            await _articleBonusLinkRepository.InsertRangeAsync(items.Where(p => p.Id == 0).ToList());
            await _articleBonusLinkRepository.UpdateRangeAsync(dbItems);

            return true;
        }
    }
}