using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Models.ContentManagement
{
    public class RecipeItemListFilter : FilterModelBase
    {
	    public string Name { get; set; }

        public int? CategoryId { get; set; }
    }
}