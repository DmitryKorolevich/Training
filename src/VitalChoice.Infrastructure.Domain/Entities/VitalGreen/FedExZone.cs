using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.VitalGreen
{
    public class FedExZone : Entity
    {
        public string Company { get; set; }

        public string Address { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        public string Phone { get; set; }

        public string Website { get; set; }

        public string Contact { get; set; }

        public string StatesCovered { get; set; }

        public bool InUse { get; set; }
    }
}
