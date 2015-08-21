using System;

namespace VitalChoice.Domain.Entities.Content
{
    public class RecipeVideo: Entity
    {
	    public int IdRecipe { get; set; }

	    public string Image { get; set; }

	    public string Video { get; set; }

	    public string Text { get; set; }

		public byte Number { get; set; }
	}
}
