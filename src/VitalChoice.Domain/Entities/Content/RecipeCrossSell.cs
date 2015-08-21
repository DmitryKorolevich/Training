﻿namespace VitalChoice.Domain.Entities.Content
{
    public class RecipeCrossSell: Entity
    {
		public int IdRecipe { get; set; }

		public string Url { get; set; }

	    public string Image { get; set; }

	    public string Title { get; set; }

	    public string Subtitle { get; set; }

	    public byte Number { get; set; }
    }
}
