﻿using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Content
{
    public class RecipeQuery : QueryObject<Recipe>
    {
        public RecipeQuery WithId(int id)
        {
            And(x => x.Id == id);
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
    }
}