using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Transfer.PaymentMethod;

namespace VitalChoice.Interfaces.Services.Payments
{
    public interface IPaymentMethodService
    {
	    Task<IList<ExtendedPaymentMethod>> GetApprovedPaymentMethodsAsync();

	    Task SetStateAsync(IList<PaymentMethodsAvailability> paymentMethodsAvailability);

	    Task<PaymentMethod> GetStorefrontDefaultPaymentMenthod();
    }
}
