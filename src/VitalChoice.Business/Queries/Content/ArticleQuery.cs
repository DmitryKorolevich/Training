﻿using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Content
{
    public class ArticleQuery : QueryObject<Article>
    {
        public ArticleQuery WithId(int id)
        {
            And(x => x.Id.Equals(id));
            return this;
        }

        public ArticleQuery WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                And(x => x.Name.Contains(name));
            }
            return this;
        }

        public ArticleQuery WithIds(List<int> ids)
        {
            if (ids.Count>0)
            {
                foreach (var id in ids)
                {
                    Or(x => x.Id == id);
                }
            }
            else
            {
                And(x => 1 != 1);
            }
            return this;
        }

        public ArticleQuery NotWithIds(List<int> ids)
        {
            if (ids.Count > 0)
            {
                foreach (var id in ids)
                {
                    And(x => x.Id != id);
                }
            }
            return this;
        }

        public ArticleQuery NotDeleted()
        {
            And(x => x.StatusCode.Equals(RecordStatusCode.Active) || x.StatusCode.Equals(RecordStatusCode.NotActive));
            return this;
        }
    }
}