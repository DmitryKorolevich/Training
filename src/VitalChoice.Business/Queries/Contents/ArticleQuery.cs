using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Articles;

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
            if (!string.IsNullOrEmpty(name))
            {
                And(x => x.Name.Contains(name));
            }
            return this;
        }

        public ArticleQuery WithIds(ICollection<int> ids)
        {
            if (ids.Count > 0)
            {
                Or(x => ids.Contains(x.Id));
            }
            return this;
        }

        public ArticleQuery NotWithIds(ICollection<int> ids)
        {
            if (ids!=null && ids.Count > 0)
            {
                And(x => !ids.Contains(x.Id));
            }
            return this;
        }

        public ArticleQuery NotDeleted()
        {
            And(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);
            return this;
        }
    }
}