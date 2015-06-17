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
using VitalChoice.Infrastructure.Services;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;

namespace VitalChoice.Business.Services.Content
{
    public class RecipeService : IRecipeService
    {
        private readonly IRepositoryAsync<Recipe> recipeRepository;
        private readonly IRepositoryAsync<ContentCategory> contentCategoryRepository;
        private readonly IRepositoryAsync<ContentItem> contentItemRepository;
        private readonly IRepositoryAsync<RecipeToContentCategory> recipeToContentCategoryRepository;
        private readonly IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository;
        private readonly IRepositoryAsync<ContentTypeEntity> contentTypeRepository;
        private readonly ITtlGlobalCache templatesCache;
        private readonly ILogger logger;

        public RecipeService(IRepositoryAsync<Recipe> recipeRepository,
            IRepositoryAsync<ContentCategory> contentCategoryRepository,
            IRepositoryAsync<ContentItem> contentItemRepository,
            IRepositoryAsync<RecipeToContentCategory> recipeToContentCategoryRepository,
            IRepositoryAsync<ContentItemToContentProcessor> contentItemToContentProcessorRepository,
            IRepositoryAsync<ContentTypeEntity> contentTypeRepository, ILoggerProviderExtended loggerProvider)
        {
            this.recipeRepository = recipeRepository;
            this.contentCategoryRepository = contentCategoryRepository;
            this.contentItemRepository = contentItemRepository;
            this.recipeToContentCategoryRepository = recipeToContentCategoryRepository;
            this.contentItemToContentProcessorRepository = contentItemToContentProcessorRepository;
            this.contentTypeRepository = contentTypeRepository;
            logger = loggerProvider.CreateLoggerDefault();
        }

        public async Task<PagedList<Recipe>> GetRecipesAsync(RecipeListFilter filter)
        {
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
            query = query.WithName(filter.Name).NotDeleted();

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
					recipeRepository.EarlyRead = true; //added temporarly till ef 7 becomes stable

					sortable =
						(x) =>
							sortOrder == SortOrder.Asc
								? x.OrderBy(y => y.ContentItem.Updated)
								: x.OrderByDescending(y => y.ContentItem.Updated);
					break;
			}

			var toReturn = await recipeRepository.Query(query).Include(p => p.ContentItem).Include(p => p.RecipesToContentCategories).ThenInclude(p => p.ContentCategory).
                Include(p => p.User).ThenInclude(p => p.Profile).
                OrderBy(sortable).
                SelectPageAsync(filter.Paging.PageIndex,filter.Paging.PageItemCount);

            //VitalChoiceContext content = new VitalChoiceContext();
            //var res = (from p in content.Set<Recipe>()
            //           join o in content.Set<ContentItem>().AsNoTracking()
            //               on p.ContentItemId equals o.Id
            //           select new Recipe
            //           {
            //               Id = p.Id,
            //               ContentItem = o
            //           }).OrderBy(p => p.ContentItem.Title).ToList();


            return toReturn;
        }

        public async Task<Recipe> GetRecipeAsync(int id)
        {
            RecipeQuery query = new RecipeQuery().WithId(id).NotDeleted();
            var toReturn = (await recipeRepository.Query(query).Include(p=>p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
                Include(p=>p.RecipesToContentCategories).Include(p => p.User).ThenInclude(p => p.Profile).
                SelectAsync(false)).FirstOrDefault();
            return toReturn;
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
                dbItem = (await recipeRepository.Query(p => p.Id == model.Id).Include(p => p.ContentItem).ThenInclude(p=>p.ContentItemToContentProcessors).
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
                    && p.StatusCode != RecordStatusCode.Deleted).SelectAnyAsync();
                if (urlDublicatesExist)
                {
                    throw new AppValidationException("Url","Recipe with the same URL already exists, please use a unique URL.");
                }

                dbItem.Name = model.Name;
                dbItem.Url = model.Url;
                dbItem.FileUrl = model.FileUrl;
                dbItem.UserId = model.UserId;
                dbItem.ContentItem.Updated = DateTime.Now;
                dbItem.ContentItem.Template = model.ContentItem.Template;
                dbItem.ContentItem.Description = model.ContentItem.Description;
                dbItem.ContentItem.Title = model.ContentItem.Title;
                dbItem.ContentItem.MetaDescription = model.ContentItem.MetaDescription;
                dbItem.ContentItem.MetaKeywords = model.ContentItem.MetaKeywords;
                dbItem.ContentItem.ContentItemToContentProcessors = model.ContentItem.ContentItemToContentProcessors;

                if (model.Id == 0)
                {
                    dbItem = await recipeRepository.InsertGraphAsync(dbItem);
                }
                else
                {
                    dbItem = await recipeRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> AttachRecipeToCategoriesAsync(int id, IEnumerable<int> categoryIds)
        {
            bool toReturn = false;
            var dbItem = (await recipeRepository.Query(p => p.Id == id).Include(p=>p.RecipesToContentCategories).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                var categories = await contentCategoryRepository.Query(p => categoryIds.Contains(p.Id) && p.Type == ContentType.RecipeCategory && p.StatusCode != RecordStatusCode.Deleted).SelectAsync(false);

                List<int> forDelete = new List<int>();
                foreach (var recipeToContentCategory in dbItem.RecipesToContentCategories)
                {
                    bool delete = true;
                    foreach (var category in categories)
                    {
                        if(recipeToContentCategory.ContentCategoryId==category.Id)
                        {
                            delete = false;
                            break;
                        }
                    }
                    if(delete)
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

                foreach (var forDeleteId in forDelete)
                {
                    await recipeToContentCategoryRepository.DeleteAsync(forDeleteId);
                }
                await recipeToContentCategoryRepository.InsertRangeAsync(forAdd.Select(p => new RecipeToContentCategory()
                {
                    ContentCategoryId = p,
                    RecipeId = dbItem.Id
                }).ToList());

                toReturn = true;
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