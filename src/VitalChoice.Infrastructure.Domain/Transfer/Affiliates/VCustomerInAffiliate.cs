using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class VCustomerInAffiliate : Entity
    {
        public string Name { get; set; }

        public int Count { get; set; }
    }
}