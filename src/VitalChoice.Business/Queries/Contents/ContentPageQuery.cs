using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;

namespace VitalChoice.Business.Queries.Content
{
    public class ContentPageQuery : QueryObject<ContentPage>
    {
        public ContentPageQuery WithId(int id)
        {
            And(x => x.Id == id);
            return this;
        }

        public ContentPageQuery WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                And(x => x.Name.Contains(name));
            }
            return this;
        }

        public ContentPageQuery WithIds(ICollection<int> ids)
        {
            if (ids.Count > 0)
            {
                Or(x => ids.Contains(x.Id));
            }
            return this;
        }

        public ContentPageQuery NotWithIds(ICollection<int> ids)
        {
            if (ids.Count > 0)
            {
                And(x => !ids.Contains(x.Id));
            }
            return this;
        }

        public ContentPageQuery NotDeleted()
        {
            And(x => x.StatusCode==RecordStatusCode.Active || x.StatusCode==RecordStatusCode.NotActive);
            return this;
        }
    }
}