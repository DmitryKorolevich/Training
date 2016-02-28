using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;
using VC.Admin.Models.ContentManagement;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Validation.Models;
using System.Security.Claims;
using VitalChoice.Core.Base;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;
using VitalChoice.Interfaces.Services;
using VitalChoice.Infrastructure.Domain.Transfer;
using VC.Admin.Models.EmailTemplates;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.ContentCrossSells;
using VitalChoice.Interfaces.Services.Products;

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
        private readonly IEmailTemplateService emailTemplateService;
	    private readonly IContentCrossSellService contentCrossSellService;
	    private readonly IProductService productService;
	    private readonly ILogger logger;

        public ContentController(IMasterContentService masterContentService, ICategoryService categoryService,
            IRecipeService recipeService, IFAQService faqService, IArticleService articleService, IContentPageService contentPageService,
            IEmailTemplateService emailTemplateService, ILoggerProviderExtended loggerProvider, IContentCrossSellService contentCrossSellService, IProductService productService)
        {
            this.masterContentService = masterContentService;
            this.categoryService = categoryService;
            this.recipeService = recipeService;
            this.faqService = faqService;
            this.articleService = articleService;
            this.contentPageService = contentPageService;
            this.emailTemplateService = emailTemplateService;
	        this.contentCrossSellService = contentCrossSellService;
	        this.productService = productService;
	        this.logger = loggerProvider.CreateLoggerDefault();
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
            if (id == 0)
            {
                var now = DateTime.Now;
                return new MasterContentItemManageModel()
                {
                    Template = String.Empty,
                    ProcessorIds = new List<int>(),
                    Type = ContentType.RecipeCategory,
                    IsDefault = false,
                };
            }
            var user = HttpContext.Request.HttpContext.User.GetUserId();

            return new MasterContentItemManageModel((await masterContentService.GetMasterContentItemAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.ManageMasterTemplates)]
        public async Task<Result<int?>> UpdateMasterContentItem([FromBody]MasterContentItemManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
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
        public async Task<Result<bool>> DeleteMasterContentItem(int id, [FromBody] object model)
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
            if (!Validate(model))
                return false;
            var category = model.Convert();
            return await categoryService.UpdateCategoriesTreeAsync(category);
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<CategoryManageModel>> GetCategory(int id)
        {
            if (id == 0)
            {
                return new CategoryManageModel()
                {
                    Template=String.Empty,
                };
            }
            return new CategoryManageModel((await categoryService.GetCategoryAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<CategoryManageModel>> UpdateCategory([FromBody]CategoryManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }
            item = await categoryService.UpdateCategoryAsync(item);

            return new CategoryManageModel(item);
        }
        
        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<bool>> DeleteCategory(int id, [FromBody] object model)
        {
            return await categoryService.DeleteCategoryAsync(id);
        }


        #endregion

        #region EmailTemplates

        [HttpPost]
        public async Task<Result<PagedList<EmailTemplateListItemModel>>> GetEmailTemplates([FromBody]FilterBase filter)
        {
            var result = await emailTemplateService.GetEmailTemplatesAsync(filter);
            var toReturn = new PagedList<EmailTemplateListItemModel>
            {
                Items = result.Items.Select(p => new EmailTemplateListItemModel(p)).ToList(),
                Count = result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<EmailTemplateManageModel>> GetEmailTemplate(int id)
        {
            if (id == 0)
            {
                var model = new EmailTemplateManageModel()
                {
                    Subject=string.Empty,
                    Template = string.Empty,
                };

                return model;
            }
            return new EmailTemplateManageModel(await emailTemplateService.GetEmailTemplateAsync(id));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<EmailTemplateManageModel>> UpdateEmailTemplate([FromBody]EmailTemplateManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
            var sUserId = Request.HttpContext.User.GetUserId();
            int userId;
            if (Int32.TryParse(sUserId, out userId))
            {
                item.UserId = userId;
            }

            item = await emailTemplateService.UpdateEmailTemplateAsync(item);

            return new EmailTemplateManageModel(item);
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<bool>> DeleteEmailTemplate(int id, [FromBody] object model)
        {
            return await emailTemplateService.DeleteEmailTemplateAsync(id);
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
	    public async Task<Result<Dictionary<string,string>>> GetRecipeSettings()
	    {
		    var result = await recipeService.GetRecipeSettingsAsync();

		    return result.ToDictionary(x => x.Key, y => y.Value);
	    }

	    [HttpGet]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<RecipeManageModel>> GetRecipe(int id)
        {
            if (id == 0)
            {
	            var model = new RecipeManageModel()
                {
                    Template = string.Empty,
                    CategoryIds = new List<int>(),
                    RecipesToProducts = new List<RecipeToProduct>()
				};

	            for (short i = 0; i < RecipeManageModel.CrossSellRecipesMaxCount; i++)
	            {
					model.CrossSellRecipes.Add(new CrossSellRecipeModel { Number = (byte)(i + 1) });
	            }
				for (short i = 0; i < RecipeManageModel.RelatedRecipesMaxCount; i++)
				{
					model.RelatedRecipes.Add(new RelatedRecipeModel { Number = (byte)(i + 1) });
				}

				return model;
            }
            return new RecipeManageModel(await recipeService.GetRecipeAsync(id));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<RecipeManageModel>> UpdateRecipe([FromBody]RecipeManageModel model)
        {
			model.CrossSellRecipes = model.CrossSellRecipes.Where(x => x.InUse).ToList();
			model.RelatedRecipes = model.RelatedRecipes.Where(x => x.InUse).ToList();

            if (!Validate(model))
                return null;
            var item = model.Convert();
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
        public async Task<Result<bool>> DeleteRecipe(int id, [FromBody] object model)
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
            if (id == 0)
            {
                var now = DateTime.Now;
                return new FAQManageModel()
                {
                    Template = String.Empty,
                    CategoryIds = new List<int>(),
                };
            }
            return new FAQManageModel((await faqService.GetFAQAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<FAQManageModel>> UpdateFAQ([FromBody]FAQManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
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
        public async Task<Result<bool>> DeleteFAQ(int id, [FromBody] object model)
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
            if(id==0)
            {
                var now = DateTime.Now;
                return new ArticleManageModel()
                {
                    PublishedDate = new DateTime(now.Year, now.Month, now.Day),
                    Template=String.Empty,
                    CategoryIds=new List<int>(),
                    ArticlesToProducts =new List<ArticleToProduct>(),
                };
            }
            return new ArticleManageModel((await articleService.GetArticleAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<ArticleManageModel>> UpdateArticle([FromBody]ArticleManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
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
        public async Task<Result<bool>> DeleteArticle(int id, [FromBody] object model)
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
            if (id == 0)
            {
                var now = DateTime.Now;
                return new ContentPageManageModel()
                {
                    Template = String.Empty,
                    CategoryIds = new List<int>(),
                    StatusCode=RecordStatusCode.NotActive,
                    Assigned=CustomerTypeCode.All,
                };
            }
            return new ContentPageManageModel((await contentPageService.GetContentPageAsync(id)));
        }

        [HttpPost]
        [AdminAuthorize(PermissionType.Content)]
        public async Task<Result<ContentPageManageModel>> UpdateContentPage([FromBody]ContentPageManageModel model)
        {
            if (!Validate(model))
                return null;
            var item = model.Convert();
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
        public async Task<Result<bool>> DeleteContentPage(int id, [FromBody] object model)
        {
            return await contentPageService.DeleteContentPageAsync(id);
        }

		#endregion

		#region

	    private async Task<ContentCrossSellItemModel> PopulateContentCrossSellItemModel(ContentCrossSell item)
	    {
		    var sku = await productService.GetSkuAsync(item.IdSku);

		    return new ContentCrossSellItemModel()
		    {
			    Type = item.Type,
			    Title = item.Title,
			    ImageUrl = item.ImageUrl,
			    IdSku = item.IdSku,
			    Id = item.Id,
				SkuCode = sku.Code,
				RetailPrice = sku.Price,
				WholesalePrice = sku.WholesalePrice
		    };
	    }

		private async Task<ContentCrossSellModel> PopulateContentCrossSellModel(IList<ContentCrossSell> contentCrossSells, ContentCrossSellType type)
		{
			var model = new ContentCrossSellModel() { Type = type};

			for (var i = 0; i < ContentConstants.CONTENT_CROSS_SELL_LIMIT; i++)
			{
				var item = contentCrossSells.SingleOrDefault(x => x.Order == i + 1);
				var modelItem = item != null ? await PopulateContentCrossSellItemModel(item) : new ContentCrossSellItemModel() { Type = type };

				model.Items.Add(modelItem);
			}

			return model;
		}

		[HttpGet]
	    [AdminAuthorize(PermissionType.Content)]
	    public async Task<Result<ContentCrossSellModel>> GetContentCrossSells(ContentCrossSellType id)
	    {
		    var contentCrossSells = await contentCrossSellService.GetContentCrossSellsAsync(id);

		    return await PopulateContentCrossSellModel(contentCrossSells, id);
	    }

	    [HttpPost]
		[AdminAuthorize(PermissionType.Content)]
		public async Task<Result<ContentCrossSellModel>> UpdateContentCrossSells([FromBody]ContentCrossSellModel contentCrossSellsModel)
		{
			if (!Validate(contentCrossSellsModel))
				return null;

			var sUserId = Request.HttpContext.User.GetUserId();
			var userId = Convert.ToInt32(sUserId);

			var alreadyExists = await contentCrossSellService.GetContentCrossSellsAsync(contentCrossSellsModel.Type);
			var itemsToStore = new List<ContentCrossSell>();
		    for (var i = 0; i < contentCrossSellsModel.Items.Count; i++)
		    {
			    ContentCrossSell item;
			    if (contentCrossSellsModel.Items[i].Id > 0)
			    {
				    item = alreadyExists.Single(x => x.Id == contentCrossSellsModel.Items[i].Id);
				    item.Title = contentCrossSellsModel.Items[i].Title;
				    item.ImageUrl = contentCrossSellsModel.Items[i].ImageUrl;
				    item.IdSku = contentCrossSellsModel.Items[i].IdSku;
				    item.IdEditedBy = userId;
				    item.DateEdited = DateTime.Now;
				    item.Order = i + 1;
			    }
			    else
			    {
				    item = new ContentCrossSell()
				    {
					    Type = contentCrossSellsModel.Items[i].Type,
					    Title = contentCrossSellsModel.Items[i].Title,
					    ImageUrl = contentCrossSellsModel.Items[i].ImageUrl,
					    IdSku = contentCrossSellsModel.Items[i].IdSku,
					    IdEditedBy = userId,
					    DateCreated = DateTime.Now,
					    DateEdited = DateTime.Now,
						Order = i + 1
				    };
			    }

			    itemsToStore.Add(item);
			}
			
			var contentCrossSells =  await contentCrossSellService.AddUpdateContentCrossSellsAsync(itemsToStore);

			return await PopulateContentCrossSellModel(contentCrossSells, contentCrossSellsModel.Type);
		}

		#endregion
	}
}