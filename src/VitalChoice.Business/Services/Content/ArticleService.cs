using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.Logging;
using Templates;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Content
{
    public class ArticleService : IArticleService
    {
        private readonly IRepositoryAsync<Article> articleRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<ArticleToContentCategory> articleToContentCategoryRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IEcommerceRepositoryAsync<ProductOptionType> optionTypeRepository;
        private readonly IRepositoryAsync<ArticleToProduct> _articleToProductRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public ArticleService(IEcommerceRepositoryAsync<ProductOptionType> optionTypeRepository,
            IRepositoryAsync<Article> articleRepository, IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<ArticleToContentCategory> articleToContentCategoryRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IRepositoryAsync<ArticleToProduct> articleToProductRepository,
            IEcommerceRepositoryAsync<Product> productRepository,
            ILoggerProviderExtended logger)
        {
            this.optionTypeRepository = optionTypeRepository;

            this.articleRepository = articleRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.articleToContentCategoryRepository = articleToContentCategoryRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this._articleToProductRepository = articleToProductRepository;
            this._productRepository = productRepository;
            this.logger = logger.CreateLoggerDefault();
        }

        public async Task<PagedList<Article>> GetArticlesAsync(ArticleItemListFilter filter)
        {
            ArticleQuery query = new ArticleQuery();
            List<int> ids = null;
            if(filter.CategoryId.HasValue)
            {
                if (filter.CategoryId.Value != -1)
                {
                    ids = (await articleToContentCategoryRepository.Query(p => p.ContentCategoryId == filter.CategoryId.Value).SelectAsync(false)).Select(p => p.ArticleId).ToList();
                    if(ids.Count==0)
                    {
                        return new PagedList<Article>()
                        {
                            Count=0,
                            Items =new List<Article>(),
                        };
                    }
                    query = query.WithIds(ids);
                }
                else
                {
                    ids = (await articleToContentCategoryRepository.Query().SelectAsync(false)).Select(p => p.ArticleId).Distinct().ToList();
                    query = query.NotWithIds(ids);
                }
            }
            query=query.WithName(filter.Name).NotDeleted();

			Func<IQueryable<Article>, IOrderedQueryable<Article>> sortable = x => x.OrderBy(y => y.Name);
			var sortOrder = filter.Sorting.SortOrder;
	        switch (filter.Sorting.Path)
	        {
		        case ArticleSortPath.Title:
			        sortable =
				        (x) =>
					        sortOrder == SortOrder.Asc
						        ? x.OrderBy(y => y.Name)
						        : x.OrderByDescending(y => y.Name);
			        break;
		        case ArticleSortPath.Url:
			        sortable =
				        (x) =>
					        sortOrder == SortOrder.Asc
						        ? x.OrderBy(y => y.Url)
						        : x.OrderByDescending(y => y.Url);
			        break;
		        case ArticleSortPath.Updated:
			        articleRepository.EarlyRead = true; //added temporarly till ef 7 becomes stable

			        sortable =
				        (x) =>
					        sortOrder == SortOrder.Asc
						        ? x.OrderBy(y => y.ContentItem.Updated)
						        : x.OrderByDescending(y => y.ContentItem.Updated);
			        break;
	        }

	        var toReturn = await articleRepository.Query(query).Include(p=>p.ContentItem).Include(p => p.ArticlesToContentCategories).ThenInclude(p => p.ContentCategory).OrderBy(sortable).
                Include(p => p.User).ThenInclude(p => p.Profile).
                SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

        public async Task<Article> GetArticleAsync(int id)
        {
            ArticleQuery query = new ArticleQuery().WithId(id).NotDeleted();
            var toReturn = (await articleRepository.Query(query).Include(p=>p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
                Include(p=>p.ArticlesToContentCategories).Include(p => p.User).ThenInclude(p => p.Profile).
                Include(p => p.ArticlesToProducts).
                SelectAsync(false)).FirstOrDefault();

            var productIds = toReturn.ArticlesToProducts.Select(p => p.IdProduct).ToList();
            if (productIds.Count > 0)
            {
                var shortProducts = (await _productRepository.Query(p => productIds.Contains(p.Id) && p.StatusCode != RecordStatusCode.Deleted)
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
                var contentType = (await contentTypeRepository.Query(p => p.Id == (int)ContentType.Article).SelectAsync()).FirstOrDefault();
                if (contentType == null || !contentType.DefaultMasterContentItemId.HasValue)
                {
                    throw new Exception("The default master template isn't confugurated. Please contact support.");
                }
                dbItem.MasterContentItemId = contentType.DefaultMasterContentItemId.Value;
            }
            else
            {
                dbItem = (await articleRepository.Query(p => p.Id == model.Id).Include(p => p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
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
                var urlDublicatesExist = await articleRepository.Query(p => p.Url == model.Url && p.Id != dbItem.Id
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
                    dbItem = await articleRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    dbItem = await articleRepository.UpdateAsync(dbItem);
                }

                foreach (var item in articlesToProducts)
                {
                    item.Id = 0;
                    item.IdArticle = dbItem.Id;
                }
                var dbProducts = await _articleToProductRepository.Query(c => c.IdArticle == dbItem.Id).SelectAsync();
                await _articleToProductRepository.DeleteAllAsync(dbProducts);
                await _articleToProductRepository.InsertRangeAsync(articlesToProducts);
                dbItem.ArticlesToProducts = articlesToProducts;
            }

            return dbItem;
        }

        public async Task<bool> AttachArticleToCategoriesAsync(int id, IEnumerable<int> categoryIds)
        {
            bool toReturn = false;
            var dbItem = (await articleRepository.Query(p => p.Id == id).Include(p=>p.ArticlesToContentCategories).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                var categories =
                    await
                        contentCategoryRepository.Query(
                            p =>
                                categoryIds.Contains(p.Id) && p.Type == ContentType.ArticleCategory &&
                                p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);

                List<int> forDelete = new List<int>();
                foreach (var articleToContentCategory in dbItem.ArticlesToContentCategories)
                {
                    bool delete = true;
                    foreach (var category in categories)
                    {
                        if(articleToContentCategory.ContentCategoryId==category.Id)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if(delete)
                    {
                        forDelete.Add(articleToContentCategory.Id);
                    }
                }

                List<int> forAdd = new List<int>();
                foreach (var category in categories)
                {
                    bool add = true;
                    foreach (var articleToContentCategory in dbItem.ArticlesToContentCategories)
                    {
                        if (articleToContentCategory.ContentCategoryId == category.Id)
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
                    await articleToContentCategoryRepository.DeleteAsync(forDeleteId);
                }
                await articleToContentCategoryRepository.InsertRangeAsync(forAdd.Select(p => new ArticleToContentCategory()
                {
                    ContentCategoryId = p,
                    ArticleId = dbItem.Id
                }).ToList());

                toReturn = true;
            }

            return toReturn;
        }

        public async Task<bool> DeleteArticleAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await articleRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await articleRepository.UpdateAsync(dbItem);

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