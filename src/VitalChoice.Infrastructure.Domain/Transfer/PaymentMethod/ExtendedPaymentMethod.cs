using VitalChoice.Infrastructure.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Transfer.PaymentMethod
{
    public class ExtendedPaymentMethod: Ecommerce.Domain.Entities.Payment.PaymentMethod
    {
	    public AdminProfile AdminProfile { get; set; }
    }
}
