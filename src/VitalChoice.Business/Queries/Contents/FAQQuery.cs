using System;
using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Faq;

namespace VitalChoice.Business.Queries.Content
{
    public class FAQQuery : QueryObject<FAQ>
    {
        public FAQQuery WithId(int id)
        {
            And(x => x.Id == id);
            return this;
        }

        public FAQQuery WithIdOld(int idOld)
        {
            Add(x => x.IdOld == idOld);

            return this;
        }

        public FAQQuery WithName(string name)
        {
            if (!String.IsNullOrEmpty(name))
            {
                And(x => x.Name.Contains(name));
            }
            return this;
        }

        public FAQQuery WithIds(ICollection<int> ids)
        {
            if (ids.Count>0)
            {
                foreach (var id in ids)
                {
                    Or(x => ids.Contains(x.Id));
                }
            }
            return this;
        }

        public FAQQuery NotWithIds(ICollection<int> ids)
        {
            if (ids.Count > 0)
            {
                And(x => !ids.Contains(x.Id));
            }
            return this;
        }

        public FAQQuery NotDeleted()
        {
            And(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);
            return this;
        }
    }
}