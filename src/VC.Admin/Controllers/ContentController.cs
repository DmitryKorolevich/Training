using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;
using VitalChoice.Validation.Models;
using System.Security.Claims;
using VitalChoice.Business.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Validation.Base;

namespace VC.Admin.Controllers
{
    public class ContentController : BaseApiController
    {
        private readonly IMasterContentService masterContentService;
        private readonly ICategoryService categoryService;
        private readonly IRecipeService recipeService;
        private readonly IFAQService faqService;
        private readonly IArticleService articleService;
        private readonly IContentPageService contentPageService;
        private readonly ILogger logger;

        public ContentController(IMasterContentService masterContentService, ICategoryService categoryService,
            IRecipeService recipeService, IFAQService faqService, IArticleService articleService, IContentPageService contentPageService)
        {
            this.masterContentService = masterContentService;
            this.categoryService = categoryService;
            this.recipeService = recipeService;
            this.faqService = faqService;
            this.articleService = articleService;
            this.contentPageService = contentPageService;
            this.logger = LoggerService.GetDefault();
        }
        
        #region MasterContent

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<IEnumerable<MasterContentItemListItemModel>>> GetMasterContentItems([FromBody]MasterContentItemListFilter filter)
        {
            return (await masterContentService.GetMasterContentItemsAsync(filter)).Select(p=>new MasterContentItemListItemModel(p)).ToList();
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.ManageMasterTemplates)]
        public async Task<Result<MasterContentItemManageModel>> GetMasterContentItem(int id)
        {
            var user = Context.Request.HttpContext.User.GetUserId();

            return new MasterContentItemManageModel((await masterContentService.GetMasterContentItemAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.ManageMasterTemplates)]
        public async Task<Result<int?>> UpdateMasterContentItem([FromBody]MasterContentItemManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if(Int32.TryParse(sUserId,out userId))
            {
                item.UserId = userId;
            }

            item = await masterContentService.UpdateMasterContentItemAsync(item);

            return item?.Id;
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.ManageMasterTemplates)]
        public async Task<Result<bool>> DeleteMasterContentItem(int id)
        {
            return await masterContentService.DeleteMasterContentItemAsync(id);
        }

        #endregion

        #region Categories

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<CategoryTreeItemModel>> GetCategoriesTree([FromBody]CategoryTreeFilter filter)
        {
            var result = await categoryService.GetCategoriesTreeAsync(filter);

            return new CategoryTreeItemModel(result);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<bool>> UpdateCategoriesTree([FromBody]CategoryTreeItemModel model)
        {
            var category = ConvertWithValidate(model);
            if (category == null)
                return false;
            
            return await categoryService.UpdateCategoriesTreeAsync(category);
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<CategoryManageModel>> GetCategory(int id)
        {
            return new CategoryManageModel((await categoryService.GetCategoryAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<CategoryManageModel>> UpdateCategory([FromBody]CategoryManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await categoryService.UpdateCategoryAsync(item);

            return new CategoryManageModel(item);
        }
        
        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<bool>> DeleteCategory(int id)
        {
            return await categoryService.DeleteCategoryAsync(id);
        }


        #endregion

        #region Recipes

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<PagedList<RecipeListItemModel>>> GetRecipes([FromBody]RecipeListFilter filter)
        {
            var result = await recipeService.GetRecipesAsync(filter);
            var toReturn = new PagedList<RecipeListItemModel>
            {
                Items = result.Items.Select(p => new RecipeListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<RecipeManageModel>> GetRecipe(int id)
        {
            return new RecipeManageModel((await recipeService.GetRecipeAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<RecipeManageModel>> UpdateRecipe([FromBody]RecipeManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;
            
            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }

            item = await recipeService.UpdateRecipeAsync(item);
            await recipeService.AttachRecipeToCategoriesAsync(item.Id, model.CategoryIds);

            return new RecipeManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<bool>> DeleteRecipe(int id)
        {
            return await recipeService.DeleteRecipeAsync(id);
        }

        #endregion

        #region FAQs

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<PagedList<FAQListItemModel>>> GetFAQs([FromBody]FAQListFilter filter)
        {
            var result = await faqService.GetFAQsAsync(filter);
            var toReturn = new PagedList<FAQListItemModel>
            {
                Items = result.Items.Select(p => new FAQListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<FAQManageModel>> GetFAQ(int id)
        {
            return new FAQManageModel((await faqService.GetFAQAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<FAQManageModel>> UpdateFAQ([FromBody]FAQManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }

            item = await faqService.UpdateFAQAsync(item);
            await faqService.AttachFAQToCategoriesAsync(item.Id, model.CategoryIds);

            return new FAQManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<bool>> DeleteFAQ(int id)
        {
            return await faqService.DeleteFAQAsync(id);
        }

        #endregion

        #region Articles

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<PagedList<ArticleListItemModel>>> GetArticles([FromBody]ArticleItemListFilter filter)
        {
            var result = await articleService.GetArticlesAsync(filter);
            var toReturn = new PagedList<ArticleListItemModel>
            {
                Items = result.Items.Select(p => new ArticleListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<ArticleManageModel>> GetArticle(int id)
        {
            return new ArticleManageModel((await articleService.GetArticleAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<ArticleManageModel>> UpdateArticle([FromBody]ArticleManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }

            item = await articleService.UpdateArticleAsync(item);
            await articleService.AttachArticleToCategoriesAsync(item.Id, model.CategoryIds);

            return new ArticleManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<bool>> DeleteArticle(int id)
        {
            return await articleService.DeleteArticleAsync(id);
        }

        #endregion

        #region ContentPages

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<PagedList<ContentPageListItemModel>>> GetContentPages([FromBody]ContentPageListFilter filter)
        {
            var result = await contentPageService.GetContentPagesAsync(filter);
            var toReturn = new PagedList<ContentPageListItemModel>
            {
                Items = result.Items.Select(p => new ContentPageListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<ContentPageManageModel>> GetContentPage(int id)
        {
            return new ContentPageManageModel((await contentPageService.GetContentPageAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<ContentPageManageModel>> UpdateContentPage([FromBody]ContentPageManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }

            item = await contentPageService.UpdateContentPageAsync(item);
            await contentPageService.AttachContentPageToCategoriesAsync(item.Id, model.CategoryIds);

            return new ContentPageManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<bool>> DeleteContentPage(int id)
        {
            return await contentPageService.DeleteContentPageAsync(id);
        }

        #endregion

    }
}