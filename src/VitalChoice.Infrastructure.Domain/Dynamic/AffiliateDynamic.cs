using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class AffiliateDynamic : MappedObject
    {
        public AffiliateDynamic()
        {
        }

        public string Name { get; set; }

        public decimal MyAppBalance { get; set; }

        public decimal CommissionFirst { get; set; }

        public decimal CommissionAll { get; set; }

        public int IdCountry { get; set; }

        public int? IdState { get; set; }

        public string County { get; set; }

        public string Email { get; set; }
    }
}
