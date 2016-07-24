using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.CreditCards.Entities
{
    public class PaymentMethodExport : Entity
    {
        public byte[] CreditCardNumber { get; set; }
    }
}