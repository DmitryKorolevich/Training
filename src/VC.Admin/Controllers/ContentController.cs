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
        private readonly IRecipeService recipeService;
        private readonly ICategoryService categoryService;
        private readonly ILogger logger;

        public ContentController(IGeneralContentService generalContentService, IMasterContentService masterContentService, IRecipeService recipeService,
            ICategoryService categoryService)
        {
            this.generalContentService = generalContentService;
            this.masterContentService = masterContentService;
            this.recipeService = recipeService;
            this.categoryService = categoryService;
            this.logger = LoggerService.GetDefault();
        }

        #region Common

        [HttpGet]
        public async Task<Result<IEnumerable<ContentTypeEntity>>> GetContentTypesAsync()
        {
            return (await generalContentService.GetContentTypesAsync()).ToList();
        }

        [HttpGet]
        public async Task<Result<IEnumerable<ContentProcessor>>> GetContentProcessorsAsync()
        {
            return (await generalContentService.GetContentProcessorsAsync()).ToList();
        }

        #endregion

        #region MasterContent

        [HttpPost]
        public async Task<Result<IEnumerable<MasterContentItemListItemModel>>> GetMasterContentItemsAsync([FromBody]MasterContentItemListFilter filter)
        {
            return (await masterContentService.GetMasterContentItemsAsync(filter.Type, filter.Status)).Select(p=>new MasterContentItemListItemModel(p)).ToList();
        }

        [HttpGet]
        public async Task<Result<MasterContentItemManageModel>> GetMasterContentItemAsync(int id)
        {
            return new MasterContentItemManageModel((await masterContentService.GetMasterContentItemAsync(id)));
        }

        [HttpPut]
        public async Task<Result<int?>> UpdateMasterContentItemAsync([FromBody]MasterContentItemManageModel model, int? id = null)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item.Id = id.HasValue ? id.Value : 0;
            item = await masterContentService.UpdateMasterContentItemAsync(item);

            return item?.Id;
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteMasterContentItemAsync(int id)
        {
            return await masterContentService.DeleteMasterContentItemAsync(id);
        }

        #endregion

        #region Recipes

        [HttpPost]
        public async Task<Result<PagedModelList<RecipeListItemModel>>> GetRecipesAsync([FromBody]RecipeItemListFilter filter)
        {
            var result = await recipeService.GetRecipesAsync(filter.Name, filter.Paging.PageIndex, filter.Paging.PageItemCount);
            var toReturn = new PagedModelList<RecipeListItemModel>
            {
                Items = result.Items.Select(p=>new RecipeListItemModel(p)).ToList(),
                TotalItemCount= result.Count,
            };

            return toReturn;
        }

        [HttpGet]
        public async Task<Result<RecipeManageModel>> GetRecipeAsync(int id)
        {
            return new RecipeManageModel((await recipeService.GetRecipeAsync(id)));
        }

        [HttpPost]
        public async Task<Result<int?>> UpdateRecipeAsync([FromBody]RecipeManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;
            
            item = await recipeService.UpdateRecipeAsync(item);

            return item?.Id;
        }

        [HttpPost]
        public async Task<Result<bool>> AttachRecipeToCategoriesAsync(int id,[FromBody]IEnumerable<int> categoryIds)
        {
            return await recipeService.AttachRecipeToCategoriesAsync(id, categoryIds);
        }

        [HttpPost]
        public async Task<Result<bool>> DeleteRecipeAsync(int id)
        {
            return await recipeService.DeleteRecipeAsync(id);
        }

        #endregion

        #region Categories

        [HttpPost]
        public async Task<Result<CategoryTreeItemModel>> GetCategoriesTreeAsync([FromBody]CategoryTreeFilter filter)
        {
            var result = await categoryService.GetCategoriesTreeAsync(filter.Type, filter.Status);

            return new CategoryTreeItemModel(result);
        }

        [HttpPost]
        public async Task<Result<bool>> GetCategoriesTreeAsync([FromBody]CategoryTreeItemModel model)
        {
            var category = ConvertWithValidate(model);
            if (category == null)
                return false;
            
            return await categoryService.UpdateCategoriesTreeAsync(category);
        }

        [HttpGet]
        public async Task<Result<CategoryManageModel>> GetCategoryAsync(int id)
        {
            return new CategoryManageModel((await categoryService.GetCategoryAsync(id)));
        }

        [HttpPost]
        public async Task<Result<int?>> UpdateCategoryAsync([FromBody]CategoryManageModel model)
        {
            var item = ConvertWithValidate(model);
            if (item == null)
                return null;

            item = await categoryService.UpdateCategoryAsync(item);

            return item?.Id;
        }
        
        [HttpPost]
        public async Task<Result<bool>> DeleteCategoryAsync(int id)
        {
            return await categoryService.DeleteCategoryAsync(id);
        }


        #endregion
    }
}