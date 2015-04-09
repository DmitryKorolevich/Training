using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Content
{
    public class RecipeQuery : QueryObject<Recipe>
    {
        public RecipeQuery WithId(int id)
        {
            And(x => x.Id.Equals(id));
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

        public RecipeQuery WithCategoryId(int categoryId)
        {
            And(x => x.RecipesToContentCategories.Select(p=>p.ContentCategoryId).Contains(categoryId));

            return this;
        }

        public RecipeQuery WithIds(List<int> ids)
        {
            if (ids.Count>0)
            {
                foreach (var id in ids)
                {
                    Or(x => x.Id == id);
                }
            }
            return this;
        }

        public RecipeQuery NotDeleted()
        {
            And(x => x.StatusCode.Equals(RecordStatusCode.Active) || x.StatusCode.Equals(RecordStatusCode.NotActive));
            return this;
        }
    }
}