using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;

namespace VitalChoice.Business.Queries.Content
{
    public class MasterContentItemQuery : QueryObject<MasterContentItem>
    {
        public MasterContentItemQuery WithType(ContentType? type=null)
        {
            if (type.HasValue)
            {
                var typeInt = (int)type.Value;
                Add(x => x.TypeId == typeInt);
            }

            return this;
        }

        public MasterContentItemQuery WithId(int id)
        {
            Add(x => x.Id==id);

            return this;
        }

        public MasterContentItemQuery NotDeleted()
        {
            Add(x => x.StatusCode == RecordStatusCode.Active || x.StatusCode == RecordStatusCode.NotActive);

            return this;
        }

        public MasterContentItemQuery WithStatus(RecordStatusCode? status)
        {
            if (status == RecordStatusCode.Active || status == RecordStatusCode.NotActive)
            {
                Add(x => x.StatusCode == status.Value);
            }

            return this;
        }
    }
}