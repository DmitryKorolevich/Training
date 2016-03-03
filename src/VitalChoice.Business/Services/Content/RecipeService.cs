using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Content;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Infrastructure.UnitOfWork;
using System.Threading;
using VitalChoice.Business.Queries.Contents;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Data.Context;
using VitalChoice.Data.Services;
using VitalChoice.Data.Transaction;
using VitalChoice.Ecommerce.Cache;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;

namespace VitalChoice.Business.Services.Content
{
    public class RecipeService : IRecipeService
    {
        private readonly IRepositoryAsync<Recipe> recipeRepository;
	    private readonly IRepositoryAsync<RecipeCrossSell> _crossSellRepository;
	    private readonly IRepositoryAsync<RelatedRecipe> _relatedRecipeRepository;
	    private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<RecipeToContentCategory> recipeToContentCategoryRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly IRepositoryAsync<RecipeToProduct> _recipeToProductRepository;
	    private readonly IRepositoryAsync<RecipeDefaultSetting> _recipeSettingRepository;
	    private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly ITtlGlobalCache templatesCache;
	    private readonly ILogger logger;
        private readonly ITransactionAccessor<VitalChoiceContext> _transactionAccessor;
        private readonly IObjectLogItemExternalService _objectLogItemExternalService;

        public RecipeService(IRepositoryAsync<Recipe> recipeRepository, IRepositoryAsync<RecipeCrossSell> crossSellRepository, IRepositoryAsync<RelatedRecipe> relatedRecipeRepository,
			IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<RecipeToContentCategory> recipeToContentCategoryRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository,
            IRepositoryAsync<RecipeToProduct> recipeToProductRepository,
			IRepositoryAsync<RecipeDefaultSetting> recipeSettingRepository,
			IEcommerceRepositoryAsync<Product> productRepository,
            ILoggerProviderExtended loggerProvider, 
            ITtlGlobalCache templatesCache,
            ITransactionAccessor<VitalChoiceContext> transactionAccessor,
            IObjectLogItemExternalService objectLogItemExternalService)
        {
            this.recipeRepository = recipeRepository;
	        _crossSellRepository = crossSellRepository;
	        _relatedRecipeRepository = relatedRecipeRepository;
	        this.contentCategoryRepository = contentCategoryRepository;
            this.recipeToContentCategoryRepository = recipeToContentCategoryRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            this._recipeToProductRepository = recipeToProductRepository;
			_recipeSettingRepository = recipeSettingRepository;
	        this._productRepository = productRepository;
            this.templatesCache = templatesCache;
            _transactionAccessor = transactionAccessor;
            _objectLogItemExternalService = objectLogItemExternalService;
            logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<PagedList<Recipe>> GetRecipesAsync(RecipeListFilter filter)
        {
            PagedList<Recipe> toReturn;
            RecipeQuery query = new RecipeQuery();
            List<int> ids = null;
            if (filter.CategoryId.HasValue)
            {
                if (filter.CategoryId.Value != -1)
                {
                    ids = (await recipeToContentCategoryRepository.Query(p => p.ContentCategoryId == filter.CategoryId.Value).SelectAsync(false)).Select(p => p.RecipeId).ToList();
                    if (ids.Count == 0)
                    {
                        return new PagedList<Recipe>()
                        {
                            Count = 0,
                            Items = new Recipe[0],
                        };
                    }
                    query = query.WithIds(ids);
                }
                else
                {
                    ids = (await recipeToContentCategoryRepository.Query().SelectAsync(false)).Select(p => p.RecipeId).Distinct().ToList();
                    query = query.NotWithIds(ids);
                }
            }
            query = query.WithName(filter.Name).AssignedToProduct(filter.ProductId).NotDeleted();

            Func<IQueryable<Recipe>, IOrderedQueryable<Recipe>> sortable = x => x.OrderBy(y => y.Name);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case RecipeSortPath.Title:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Name)
                                : x.OrderByDescending(y => y.Name);
                    break;
                case RecipeSortPath.Url:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Url)
                                : x.OrderByDescending(y => y.Url);
                    break;
                case RecipeSortPath.Updated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.ContentItem.Updated)
                                : x.OrderByDescending(y => y.ContentItem.Updated);
                    break;
                case RecipeSortPath.Created:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.ContentItem.Created)
                                : x.OrderByDescending(y => y.ContentItem.Created);
                    break;
            }


            var resultQuery = recipeRepository.Query(query).Include(x => x.RecipesToProducts).Include(p => p.ContentItem).Include(p => p.RecipesToContentCategories).ThenInclude(p => p.ContentCategory).
                Include(p => p.User).ThenInclude(p => p.Profile).
                OrderBy(sortable);
            if (filter.Paging != null)
            {
                toReturn = await resultQuery.SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            }
            else
            {
                var items = await resultQuery.SelectAsync(false);
                toReturn = new PagedList<Recipe>(items,items.Count);
            }

            return toReturn;
        }

        public async Task<Recipe> GetRecipeAsync(int id)
        {
            RecipeQuery query = new RecipeQuery().WithId(id).NotDeleted();
	        var toReturn = (await recipeRepository.Query(query).Include(p => p.RelatedRecipes).
		        Include(p => p.CrossSells).
		        Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
		        Include(p => p.RecipesToContentCategories).
		        Include(p => p.User).ThenInclude(p => p.Profile).
		        Include(p => p.RecipesToProducts).
		        SelectAsync(false)).FirstOrDefault();

            var productIds = toReturn?.RecipesToProducts.Select(p => p.IdProduct).ToList();
	        if (productIds?.Count > 0)
	        {
		        var shortProducts =
			        (await _productRepository.Query(p => productIds.Contains(p.Id) && p.StatusCode != (int)RecordStatusCode.Deleted)
				        .SelectAsync(false)).Select(p => new ShortProductInfo(p)).ToList();
		        foreach (var product in toReturn.RecipesToProducts)
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

	    public async Task<IList<RecipeDefaultSetting>> GetRecipeSettingsAsync()
	    {
		    return await _recipeSettingRepository.Query().SelectAsync(false);
	    }

	    public async Task<Recipe> UpdateRecipeAsync(Recipe model)
        {
            Recipe dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new Recipe();
                dbItem.StatusCode = RecordStatusCode.Active;
                dbItem.ContentItem = new ContentItem();
                dbItem.ContentItem.Created = DateTime.Now;
                dbItem.ContentItem.ContentItemToContentProcessors = new List<ContentItemToContentProcessor>();

                //set predefined master
                var contentType = (await contentTypeRepository.Query(p => p.Id == (int)ContentType.Recipe).SelectAsync()).FirstOrDefault();
                if (contentType == null || !contentType.DefaultMasterContentItemId.HasValue)
                {
                    throw new Exception("The default master template isn't confugurated. Please contact support.");
                }
                dbItem.MasterContentItemId = contentType.DefaultMasterContentItemId.Value;
            }
            else
            {
                dbItem = (await recipeRepository.Query(p => p.Id == model.Id).Include(p => p.ContentItem).ThenInclude(p => p.ContentItemToContentProcessors).
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
			    var urlDublicatesExist = await recipeRepository.Query(p => p.Url == model.Url && p.Id != dbItem.Id
			                                                               && p.StatusCode != RecordStatusCode.Deleted)
				    .SelectAnyAsync();
			    if (urlDublicatesExist)
			    {
				    throw new AppValidationException("Url", "Recipe with the same URL already exists, please use a unique URL.");
			    }

			    //what is the purpose of such strange code? both model and dbItem are of the same type. Why do we need to maintain additional object?
			    dbItem.Name = model.Name;
			    dbItem.Subtitle = model.Subtitle;
			    dbItem.YoutubeVideo = model.YoutubeVideo;
			    dbItem.YoutubeImage = model.YoutubeImage;
			    dbItem.Url = model.Url;
			    dbItem.FileUrl = model.FileUrl;
			    dbItem.UserId = model.UserId;
			    dbItem.AboutChef = model.AboutChef;
			    dbItem.Directions = model.Directions;
			    dbItem.Ingredients = model.Ingredients;
			    dbItem.ContentItem.Updated = DateTime.Now;
			    dbItem.ContentItem.Template = model.ContentItem.Template;
			    dbItem.ContentItem.Description = model.ContentItem.Description;
			    dbItem.ContentItem.Title = model.ContentItem.Title;
			    dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
			    dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
			    dbItem.ContentItem.ContentItemToContentProcessors = model.ContentItem.ContentItemToContentProcessors;

			    ICollection<RecipeToProduct> recipesToProducts = new List<RecipeToProduct>();
			    if (model.RecipesToProducts != null)
			    {
				    recipesToProducts = model.RecipesToProducts.ToList();
			    }
			    dbItem.RecipesToProducts = null;

			    using (var transaction = _transactionAccessor.BeginTransaction())
			    {
				    try
				    {

					    if (model.Id == 0)
					    {
							dbItem.RelatedRecipes = model.RelatedRecipes;
							dbItem.CrossSells = model.CrossSells;
							await recipeRepository.InsertGraphAsync(dbItem);
					    }
					    else
					    {
							var crossSellsToUpdate = model.CrossSells.ToList();
							var crossSells = await _crossSellRepository.Query(x => x.IdRecipe == dbItem.Id).SelectAsync();
						    if (crossSells.Any())
						    {
							    await _crossSellRepository.DeleteAllAsync(crossSells);
						    }
						    if (crossSellsToUpdate.Any())
						    {
								await _crossSellRepository.InsertRangeAsync(crossSellsToUpdate);
							}

							var relatedToUpdate = model.RelatedRecipes.ToList();
							var relatedRecipes = await _relatedRecipeRepository.Query(x => x.IdRecipe == dbItem.Id).SelectAsync();
						    if (relatedRecipes.Any())
						    {
							    await _relatedRecipeRepository.DeleteAllAsync(relatedRecipes);
						    }
							if (relatedToUpdate.Any())
							{
								await _relatedRecipeRepository.InsertRangeAsync(relatedToUpdate);
							}

						    await recipeRepository.UpdateAsync(dbItem);

						    dbItem.CrossSells = crossSellsToUpdate;
						    dbItem.RelatedRecipes = relatedToUpdate;
					    }

					    foreach (var item in recipesToProducts)
					    {
						    item.Id = 0;
						    item.IdRecipe = dbItem.Id;
					    }

					    var dbProducts = await _recipeToProductRepository.Query(c => c.IdRecipe == dbItem.Id).SelectAsync();
					    await _recipeToProductRepository.DeleteAllAsync(dbProducts);
					    await _recipeToProductRepository.InsertRangeAsync(recipesToProducts);
					    dbItem.RecipesToProducts = recipesToProducts;

					    transaction.Commit();

                        await _objectLogItemExternalService.LogItem(dbItem);
                    }
				    catch (Exception)
				    {
					    transaction.Rollback();
					    throw;
				    }
			    }
		    }

		    return dbItem;
        }

        public async Task<bool> AttachRecipeToCategoriesAsync(int id, IEnumerable<int> categoryIds)
        {
            bool toReturn = false;

            using (var uow = new VitalChoiceUnitOfWork())
            {
                var recipeRepository = uow.RepositoryAsync<Recipe>();
                var recipeToContentCategoryRepository = uow.RepositoryAsync<RecipeToContentCategory>();
                var dbItem = (await recipeRepository.Query(p => p.Id == id).Include(p => p.RecipesToContentCategories).SelectAsync(false)).FirstOrDefault();
                if (dbItem != null)
                {
                    var categories = await contentCategoryRepository.Query(p => categoryIds.Contains(p.Id) && p.Type == ContentType.RecipeCategory && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);

                    List<int> forDelete = new List<int>();
                    foreach (var recipeToContentCategory in dbItem.RecipesToContentCategories)
                    {
                        bool delete = true;
                        foreach (var category in categories)
                        {
                            if (recipeToContentCategory.ContentCategoryId == category.Id)
                            {
                                delete = false;
                                break;
                            }
                        }
                        if (delete)
                        {
                            forDelete.Add(recipeToContentCategory.Id);
                        }
                    }

                    List<int> forAdd = new List<int>();
                    foreach (var category in categories)
                    {
                        bool add = true;
                        foreach (var recipeToContentCategory in dbItem.RecipesToContentCategories)
                        {
                            if (recipeToContentCategory.ContentCategoryId == category.Id)
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
                    dbItem.RecipesToContentCategories = null;

                    foreach (var forDeleteId in forDelete)
                    {
                        await recipeToContentCategoryRepository.DeleteAsync(forDeleteId);
                    }
                    var items = forAdd.Select(p => new RecipeToContentCategory()
                    {
                        ContentCategoryId = p,
                        RecipeId = dbItem.Id
                    }).ToList();
                    await recipeToContentCategoryRepository.InsertRangeAsync(items);
                    
                    await uow.SaveChangesAsync(CancellationToken.None);
                    toReturn = true;
                }
            }

            return toReturn;
        }

        public async Task<bool> DeleteRecipeAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await recipeRepository.Query(p => p.Id == id && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await recipeRepository.UpdateAsync(dbItem);

                try
                {
                    await templatesCache.RemoveFromCache(dbItem.MasterContentItemId, dbItem.ContentItemId);
                }
                catch (Exception e)
                {
                    logger.LogError(e.ToString());
                }
                toReturn = true;
            }
            return toReturn;
        }
    }
}