using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.PaymentMethod
{
    public class PaymentMethodsAvailability
    {
	    public int Id { get; set; }
	    public IList<int> CustomerTypes { get; set; }

        public PaymentMethodsAvailability()
        {
            CustomerTypes = new List<int>();
        }
    }
}
