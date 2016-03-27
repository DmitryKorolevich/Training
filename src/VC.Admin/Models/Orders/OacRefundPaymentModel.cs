using VC.Admin.Models.Customer;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Orders
{
    public class OacRefundPaymentModel : BaseModel
    {
        public OacRefundPaymentModel()
        {
            PaymentMethodType = PaymentMethodType.Oac;
        }

        [Map]
        public int Id { get; set; }

        [Map]
        public AddressModel Address { get; set; }

        [Map("IdObjectType")]
        public PaymentMethodType PaymentMethodType { get; set; }
    }
}