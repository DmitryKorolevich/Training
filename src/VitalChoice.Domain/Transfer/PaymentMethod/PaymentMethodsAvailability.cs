using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.PaymentMethod
{
    public class PaymentMethodsAvailability
    {
	    public int Id { get; set; }

	    public IList<int> CustomerTypes { get; set; }
    }
}
