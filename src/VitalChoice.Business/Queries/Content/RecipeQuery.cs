using System;
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
            Add(x => x.Id.Equals(id));
            return this;
        }

        public RecipeQuery WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                Add(x => x.Name.Contains(name));
            }
            return this;
        }

        public RecipeQuery NotDeleted()
        {
            Add(x => x.StatusCode.Equals(RecordStatusCode.Active) || x.StatusCode.Equals(RecordStatusCode.NotActive));
            return this;
        }
    }
}