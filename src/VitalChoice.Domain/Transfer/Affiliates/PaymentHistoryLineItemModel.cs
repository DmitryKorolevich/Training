using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.Affiliates
{
    public class PaymentHistoryLineItemModel
    {
	    public DateTime DateCreated { get; set; }

	    public decimal Amount { get; set; }
    }
}
