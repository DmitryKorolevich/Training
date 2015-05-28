using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Content
{
    public class CategoryQuery : QueryObject<ContentCategory>
    {
        public CategoryQuery WithType(ContentType? type=null)
        {
            if (type.HasValue)
            {
                Add(x => x.Type.Equals(type));
            }

            return this;
        }

        public CategoryQuery WithId(int id)
        {
            Add(x => x.Id.Equals(id));

            return this;
        }

        public CategoryQuery WithParentId(int id)
        {
            Add(x => x.ParentId.Equals(id));

            return this;
        }

        public CategoryQuery NotDeleted()
        {
            Add(x => x.StatusCode.Equals(RecordStatusCode.Active) || x.StatusCode.Equals(RecordStatusCode.NotActive));

            return this;
        }

        public CategoryQuery WithStatus(RecordStatusCode? status)
        {
            if (status == RecordStatusCode.Active || status == RecordStatusCode.NotActive)
            {
                Add(x => x.StatusCode.Equals(status));
            }

            return this;
        }
    }
}