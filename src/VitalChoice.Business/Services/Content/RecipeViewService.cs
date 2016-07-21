using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VitalChoice.ContentProcessing.Base;
using VitalChoice.ContentProcessing.Interfaces;
using VitalChoice.Data.Repositories;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Content;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.ContentProcessing.Cache;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Data.Helpers;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Business.Services.Content
{
    public class RecipeViewService : ContentViewService<Recipe, ContentParametersModel>, IRecipeViewService
    {
        private readonly IRecipeService _recipeService;

        public RecipeViewService(ITtlGlobalCache templatesCache,
            ILoggerProviderExtended loggerProvider,
            IContentProcessorService processorService,
            IRepositoryAsync<Recipe> contentRepository,
            IObjectMapper<ContentParametersModel> mapper,
            IObjectMapperFactory mapperFactory,
            IRecipeService recipeService,
            IOptions<AppOptions> appOptions)
            : base(templatesCache, loggerProvider.CreateLogger<RecipeViewService>(), processorService, contentRepository, mapper, mapperFactory, appOptions)
        {
            _recipeService = recipeService;
        }

        protected override IQueryFluent<Recipe> BuildQuery(IQueryFluent<Recipe> query)
        {
            return query.Include(p => p.RelatedRecipes)
                .Include(p => p.CrossSells)
                .Include(p => p.MasterContentItem)
                .ThenInclude(p => p.MasterContentItemToContentProcessors)
                .ThenInclude(p => p.ContentProcessor)
                .Include(p => p.ContentItem)
                .ThenInclude(p => p.ContentItemToContentProcessors)
                .ThenInclude(p => p.ContentProcessor);
        }

        protected override async Task<Recipe> GetDataInternal(ContentParametersModel model, ContentViewContext viewContext)
        {
            Recipe entity = await base.GetDataInternal(model, viewContext);
            if(viewContext!=null && entity !=null)
            {
                var defaultSettings = await _recipeService.GetRecipeSettingsAsync();

                byte number = 1;
                while (number <= 4)
                {
                    var relatedRecipe = entity.RelatedRecipes.FirstOrDefault(p => p.Number == number);
                    if (relatedRecipe == null)
                    {
                        entity.RelatedRecipes.Add(GetDefaultRelatedRecipe(defaultSettings, number));
                    }
                    number++;
                }
                entity.RelatedRecipes = entity.RelatedRecipes.OrderBy(p => p.Number).ToList();
            }
            return entity;
        }

        private RelatedRecipe GetDefaultRelatedRecipe(ICollection<RecipeDefaultSetting> settings, byte number)
        {
            var toReturn = new RelatedRecipe();
            toReturn.Image = settings.FirstOrDefault(p=>p.Key == ContentConstants.FIELD_NAME_RELATED_RECIPE_IMAGE + number)?.Value;
            toReturn.Title = settings.FirstOrDefault(p => p.Key == ContentConstants.FIELD_NAME_RELATED_RECIPE_TITLE + number)?.Value;
            toReturn.Url = settings.FirstOrDefault(p => p.Key == ContentConstants.FIELD_NAME_RELATED_RECIPE_URL + number)?.Value;
            toReturn.Number = number;
            return toReturn;
        }
    }
}
