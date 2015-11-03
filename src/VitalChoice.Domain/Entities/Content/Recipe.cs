using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Content
{
    public class Recipe : ContentDataItem
    {
	    public Recipe()
	    {
			CrossSells = new List<RecipeCrossSell>();
			RelatedRecipes = new List<RelatedRecipe>();
        }

        public string FileUrl { get; set; }

        public virtual ICollection<RecipeToContentCategory> RecipesToContentCategories { get; set; }

        public virtual ICollection<RecipeToProduct> RecipesToProducts { get; set; }

	    public ICollection<RecipeCrossSell> CrossSells { get; set; }

	    public ICollection<RelatedRecipe> RelatedRecipes { get; set; }

	    public string AboutChef { get; set; }

	    public string Ingredients { get; set; }

	    public string Directions { get; set; }

	    public string Subtitle { get; set; }

	    public string YoutubeVideo { get; set; }

	    public string YoutubeImage { get; set; }
    }
}