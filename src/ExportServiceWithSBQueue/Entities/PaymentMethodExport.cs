using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.ExportService.Entities
{
    public class PaymentMethodExport : Entity
    {
        public byte[] CreditCardNumber { get; set; }
    }
}