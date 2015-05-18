using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Product
{
    public class GCFilter : FilterBase
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public GCType? Type { get; set; }
    }
}