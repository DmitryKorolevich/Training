using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Settings
{
    public class ObjectHistoryLogItemsFilter : FilterBase
    {
        public int IdObject { get; set; }

        public ObjectType IdObjectType { get; set; }

        public string DataReferenceId { get; set; }
    }
}
