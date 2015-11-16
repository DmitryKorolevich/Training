using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Recipes;


namespace VC.Admin.Models.ContentManagement
{
    public class RecipeListItemModel : BaseModel
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

        public string Url { get; set; }

        public ICollection<string> Categories { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string AgentId { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public RecipeListItemModel(Recipe item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Name = item.Name;
                Url = item.Url;
                StatusCode = item.StatusCode;
                if (item.RecipesToContentCategories != null)
                {
                    Categories = item.RecipesToContentCategories.OrderBy(p => p.ContentCategory.Name).Select(p => p.ContentCategory.Name).ToArray();
                }
                if (item.ContentItem!=null)
                {
                    Created = item.ContentItem.Created;
                    Updated = item.ContentItem.Updated;
                }
                if (item.User != null && item.User.Profile != null)
                {
                    AgentId = item.User.Profile.AgentId;
                }
            }
        }
    }
}