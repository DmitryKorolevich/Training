﻿using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Recipes;

namespace VitalChoice.Business.Queries.Contents
{
    public class RecipeQuery : QueryObject<Recipe>
    {
        public RecipeQuery WithId(int id)
        {
            And(x => x.Id == id);
            return this;
        }

        public RecipeQuery WithIdOld(int idOld)
        {
            Add(x => x.IdOld == idOld);

            return this;
        }

        public RecipeQuery WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                And(x => x.Name.Contains(name));
            }
            return this;
        }

        public RecipeQuery WithIds(ICollection<int> ids)
        {
            if (ids.Count>0)
            {
                Or(x => ids.Contains(x.Id));
            }
            return this;
        }

        public RecipeQuery NotWithIds(ICollection<int> ids)
        {
            if (ids.Count > 0)
            {
                And(x => !ids.Contains(x.Id));
            }
            return this;
        }

        public RecipeQuery NotDeleted()
        {
            And(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);
            return this;
        }

	    public RecipeQuery AssignedToProduct(int? productId)
	    {
		    if (productId.HasValue)
		    {
				And(x => x.RecipesToProducts.Any(y=>y.IdProduct == productId.Value));
			}

		    return this;
	    }
    }
}