using System;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Models.ContentManagement
{
    public class RecipeListItemModel : Model<Recipe, IMode>
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

        public string Url { get; set; }

	    public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public RecipeListItemModel(Recipe item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Name = item.Name;
                Url = item.Url;
                Created = item.ContentItem.Created;
                Updated = item.ContentItem.Updated;
                StatusCode = item.StatusCode;
            }
        }
    }
}