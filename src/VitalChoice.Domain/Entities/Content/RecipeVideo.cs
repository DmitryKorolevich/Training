using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.Content
{
    public class RecipeVideo: Entity
    {
	    public int IdRecipe { get; set; }

	    public string Image { get; set; }

	    public string Video { get; set; }

	    public string Text { get; set; }
    }
}
