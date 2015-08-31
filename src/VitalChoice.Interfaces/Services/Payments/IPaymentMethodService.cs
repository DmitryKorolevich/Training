using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Transfer.PaymentMethod;

namespace VitalChoice.Interfaces.Services.Payments
{
    public interface IPaymentMethodService
    {
	    Task<IList<ExtendedPaymentMethod>> GetApprovedPaymentMethodsAsync();

	    Task SetStateAsync(IList<PaymentMethodsAvailability> paymentMethodsAvailability);
    }
}
