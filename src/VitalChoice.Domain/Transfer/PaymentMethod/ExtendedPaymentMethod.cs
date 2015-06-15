using VitalChoice.Domain.Entities.Users;

namespace VitalChoice.Domain.Transfer.PaymentMethod
{
    public class ExtendedPaymentMethod: Entities.eCommerce.Payment.PaymentMethod
    {
	    public AdminProfile AdminProfile { get; set; }
    }
}
