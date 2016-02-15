using VitalChoice.Ecommerce.Domain;

namespace ExportServiceWithSBQueue.Entities
{
    public class PaymentMethodExport : Entity
    {
        public byte[] CreditCardNumber { get; set; }
    }
}