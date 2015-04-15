using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Validation.Controllers;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Validation.Helpers.GlobalFilters;
using VitalChoice.Validation.Models;
using VitalChoice.Validators.Users;
using VitalChoice.Validation.Logic;
using VitalChoice.Business.Services.Impl;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Core;
using VitalChoice.Admin.Models;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Business.Services.Contracts;
using Microsoft.Framework.Logging;
using System.Threading.Tasks;
using VitalChoice.Models.ContentManagement;
using VitalChoice.Business.Services.Contracts.Content;
using VitalChoice.Core.Infrastructure.Models;

namespace VitalChoice.Admin.Controllers
{
    public class ContentController : BaseApiController
    {
        private readonly IGeneralContentService generalContentService;
        private readonly IMasterContentService masterContentService;
        private readonly ICategoryService categoryService;
        private readonly IRecipeService recipeService;
        private readonly IFAQService faqService;
        private readonly IArticleService articleService;
        private readonly ILogger logger;

        public ContentController(IGeneralContentService generalContentService, IMasterContentService masterContentService, ICategoryService categoryService,
            IRecipeService recipeService, IFAQService faqService, IArticleService articleService)
        {
            this.generalContentService = generalContentService;
            this.masterContentService = masterContentService;
            this.categoryService = categoryService;
            this.recipeService = recipeService;
            this.faqService = faqService;
            this.articleService = articleService;
            this.logger = LoggerService.GetDefault();
        }

        #region Common

        [HttpGet]
        public async Task<Result<IEnumerable<ContentTypeEntity>>> GetContentTypes()
        {
            return (await generalContentService.GetContentTypesAsync()).ToList();
        }

        [HttpGet]
        public async Task<Result<IEnumerable<ContentProcessor>>> GetContentProcessors()
        {
            return (await generalContentService.GetContentProcessorsAsync()).ToList();
        }

        #endregion

        #region MasterContent

        [HttpPost]
        public async Task<Result<IEnumerable<MasterContentItemListItemModel>>> GetMasterContentItems([FromBody]MasterContentItemListFilter filter)
        {
            return (await masterContentService.GetMasterContentItemsAsync(filter.Type, filter.Status)).Select(p=>new MasterContentItemListItemModel(p)).ToList();
        }

        [HttpGet]
        public async Task<Result<MasterContentItemManageModel>> GetMasterContentItem(int id)
        {
            return new MasterContentItemManageModel((await masterContentService.GetMasterContentItemAsync(id)));
        }

        [HttpPost]
        public async Task<Result<int?>> UpdateMasterContentItem([FromBody]MasterContentItemManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await masterContentService.UpdateMasterContentItemAsync(item);

            return item?.Id;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteMasterContentItem(int id)
        {
            return await masterContentService.DeleteMasterContentItemAsync(id);
        }

        #endregion

        #region Categories

        [HttpPost]
        public async Task<Result<CategoryTreeItemModel>> GetCategoriesTree([FromBody]CategoryTreeFilter filter)
        {
            var result = await categoryService.GetCategoriesTreeAsync(filter.Type, filter.Status);

            return new CategoryTreeItemModel(result);
        }

        [HttpPost]
        public async Task<Result<bool>> UpdateCategoriesTree([FromBody]CategoryTreeItemModel model)
        {
            var category = ConvertWithValidate(model);
            if (category == null)
                return false;
            
            return await categoryService.UpdateCategoriesTreeAsync(category);
        }

        [HttpGet]
        public async Task<Result<CategoryManageModel>> GetCategory(int id)
        {
            return new CategoryManageModel((await categoryService.GetCategoryAsync(id)));
        }

        [HttpPost]
        public async Task<Result<CategoryManageModel>> UpdateCategory([FromBody]CategoryManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await categoryService.UpdateCategoryAsync(item);

            return new CategoryManageModel(item);
        }
        
        [HttpPost]
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            return await categoryService.DeleteCategoryAsync(id);
        }


        #endregion

        #region Recipes

        [HttpPost]
        public async Task<Result<PagedModelList<RecipeListItemModel>>> GetRecipes([FromBody]RecipeListFilter filter)
        {
            var result = await recipeService.GetRecipesAsync(filter.Name, filter.CategoryId, filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var toReturn = new PagedModelList<RecipeListItemModel>
            {
                Items = result.Items.Select(p => new RecipeListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<RecipeManageModel>> GetRecipe(int id)
        {
            return new RecipeManageModel((await recipeService.GetRecipeAsync(id)));
        }

        [HttpPost]
        public async Task<Result<RecipeManageModel>> UpdateRecipe([FromBody]RecipeManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await recipeService.UpdateRecipeAsync(item);
            await recipeService.AttachRecipeToCategoriesAsync(item.Id, model.CategoryIds);

            return new RecipeManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteRecipe(int id)
        {
            return await recipeService.DeleteRecipeAsync(id);
        }

        #endregion

        #region FAQs

        [HttpPost]
        public async Task<Result<PagedModelList<FAQListItemModel>>> GetFAQs([FromBody]FAQListFilter filter)
        {
            var result = await faqService.GetFAQsAsync(filter.Name, filter.CategoryId, filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var toReturn = new PagedModelList<FAQListItemModel>
            {
                Items = result.Items.Select(p => new FAQListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<FAQManageModel>> GetFAQ(int id)
        {
            return new FAQManageModel((await faqService.GetFAQAsync(id)));
        }

        [HttpPost]
        public async Task<Result<FAQManageModel>> UpdateFAQ([FromBody]FAQManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await faqService.UpdateFAQAsync(item);
            await faqService.AttachFAQToCategoriesAsync(item.Id, model.CategoryIds);

            return new FAQManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteFAQ(int id)
        {
            return await faqService.DeleteFAQAsync(id);
        }

        #endregion

        #region Recipes

        [HttpPost]
        public async Task<Result<PagedModelList<ArticleListItemModel>>> GetArticles([FromBody]ArticleItemListFilter filter)
        {
            var result = await articleService.GetArticlesAsync(filter.Name, filter.CategoryId, filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var toReturn = new PagedModelList<ArticleListItemModel>
            {
                Items = result.Items.Select(p => new ArticleListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<ArticleManageModel>> GetArticle(int id)
        {
            return new ArticleManageModel((await articleService.GetArticleAsync(id)));
        }

        [HttpPost]
        public async Task<Result<ArticleManageModel>> UpdateArticle([FromBody]ArticleManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await articleService.UpdateArticleAsync(item);
            await articleService.AttachArticleToCategoriesAsync(item.Id, model.CategoryIds);

            return new ArticleManageModel(item);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteArticle(int id)
        {
            return await articleService.DeleteArticleAsync(id);
        }

        #endregion
    }
}