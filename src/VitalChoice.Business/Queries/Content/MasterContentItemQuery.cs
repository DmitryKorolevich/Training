using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Content
{
    public class MasterContentItemQuery : QueryObject<MasterContentItem>
    {
        public MasterContentItemQuery WithType(ContentType? type=null)
        {
            if (type.HasValue)
            {
                Add(x => x.Type.Equals(type));
            }

            return this;
        }

        public MasterContentItemQuery WithId(int id)
        {
            Add(x => x.Id.Equals(id));

            return this;
        }

        public MasterContentItemQuery NotDeleted()
        {
            Add(x => !x.StatusCode.Equals(RecordStatusCode.Deleted));

            return this;
        }
    }
}