using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.ContentManagement;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
    [ApiValidator(typeof(RecipeManageModelValidator))]
    public class RecipeManageModel : BaseModel
    {
	    public const short RelatedRecipesMaxCount = 4;
		public const short CrossSellRecipesMaxCount = 3;

        public int Id { get; set; }

        [Localized(GeneralFieldNames.Title)]
        public string Name { get; set; }

	    public string Subtitle { get; set; }

	    [Localized(GeneralFieldNames.Url)]
        public string Url { get; set; }

        [Localized(GeneralFieldNames.Description)]
        public string Description { get; set; }

        public string FileUrl { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public int MasterContentItemId { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public IEnumerable<int> CategoryIds { get; set; }

        public IList<RecipeToProduct> RecipesToProducts { get; set; }

		public IList<CrossSellRecipeModel> CrossSellRecipes { get; set; }

		public IList<RelatedRecipeModel> RelatedRecipes { get; set; }

	    public string AboutChef { get; set; }

	    public string Ingredients  { get; set; }

	    public string Directions { get; set; }

		public string YoutubeVideo { get; set; }

		public string YoutubeImage { get; set; }

		public RecipeManageModel()
        {
			CrossSellRecipes = new List<CrossSellRecipeModel>();
			RelatedRecipes = new List<RelatedRecipeModel>();
        }

        public RecipeManageModel(Recipe item):this()
        {
            Id = item.Id;
            Name = item.Name;
            Subtitle = item.Subtitle;
			YoutubeVideo = item.YoutubeVideo;
			YoutubeImage = item.YoutubeImage;
            Url = item.Url;
            FileUrl = item.FileUrl;
            StatusCode = item.StatusCode;
            Template = item.ContentItem.Template;
            Description = item.ContentItem.Description;
            Title = item.ContentItem.Title;
            MetaKeywords = item.ContentItem.MetaKeywords;
            MetaDescription = item.ContentItem.MetaDescription;
            Created = item.ContentItem.Created;
            Updated = item.ContentItem.Updated;
	        AboutChef = item.AboutChef;
			Ingredients = item.Ingredients;
			Directions = item.Directions;
            MasterContentItemId = item.MasterContentItemId;
            ProcessorIds = item.ContentItem.ContentItemToContentProcessors != null ? item.ContentItem.ContentItemToContentProcessors.Select(p => p.ContentItemProcessorId).ToList() : new List<int>();
            if (item.RecipesToContentCategories != null)
            {
                CategoryIds = item.RecipesToContentCategories.Select(p => p.ContentCategoryId).ToList();
            }

            RecipesToProducts = item.RecipesToProducts.ToList();

	        for (short i = 0; i < CrossSellRecipesMaxCount; i++)
	        {
		        var curNumber = (byte)(i + 1);

		        var overridenItem = item.CrossSells.SingleOrDefault(x => x.Number == curNumber);
		        var model = new CrossSellRecipeModel() {Number = curNumber, InUse = false};
		        if (overridenItem != null)
		        {
			        model.Image = overridenItem.Image;
			        model.Subtitle = overridenItem.Subtitle;
			        model.Title = overridenItem.Title;
			        model.Url = overridenItem.Url;
			        model.InUse = true;
		        }

		        CrossSellRecipes.Add(model);
            }

			for (short i = 0; i < RelatedRecipesMaxCount; i++)
			{
				var curNumber = (byte)(i + 1);

				var overridenItem = item.RelatedRecipes.SingleOrDefault(x => x.Number == curNumber);
				var model = new RelatedRecipeModel() { Number = curNumber, InUse = false };
				if (overridenItem != null)
				{
					model.Image = overridenItem.Image;
					model.Title = overridenItem.Title;
					model.Url = overridenItem.Url;
					model.InUse = true;
				}

				RelatedRecipes.Add(model);
			}
		}

        public Recipe Convert()
        {
            var toReturn = new Recipe();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Subtitle = Subtitle?.Trim();
            toReturn.YoutubeImage = YoutubeImage?.Trim();
            toReturn.YoutubeVideo = YoutubeVideo?.Trim();
            toReturn.Url = Url?.Trim();
            toReturn.Url = toReturn.Url?.ToLower();
            toReturn.FileUrl = FileUrl?.Trim();
            toReturn.ContentItem = new ContentItem();
            toReturn.ContentItem.Template = Template;
            toReturn.ContentItem.Description = Description?.Trim();
            toReturn.ContentItem.Title = Title;
            toReturn.ContentItem.MetaKeywords = MetaKeywords;
            toReturn.ContentItem.MetaDescription = MetaDescription;
			toReturn.AboutChef = AboutChef;
			toReturn.Ingredients = Ingredients;
			toReturn.Directions = Directions;
			if (ProcessorIds != null)
            {
                toReturn.ContentItem.ContentItemToContentProcessors = ProcessorIds.Select(p => new ContentItemToContentProcessor()
                {
                    ContentItemProcessorId = p,
                }).ToList();
            }
            toReturn.RecipesToProducts = RecipesToProducts;

			toReturn.CrossSells = CrossSellRecipes.Where(x=>x.InUse).Select(x => new RecipeCrossSell()
			{
				Id = 0,
				IdRecipe = Id,
				Number = x.Number,
				Image = x.Image,
				Subtitle = x.Subtitle,
				Title = x.Title,
				Url = x.Url
			}).ToList();

			toReturn.RelatedRecipes = RelatedRecipes.Where(x => x.InUse).Select(x => new RelatedRecipe()
			{
				Id = 0,
				IdRecipe = Id,
				Number = x.Number,
				Image = x.Image,
				Title = x.Title,
				Url = x.Url
			}).ToList();

			return toReturn;
        }
    }
}