using System;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class PaymentHistoryLineItemModel
    {
	    public DateTime DateCreated { get; set; }

	    public decimal Amount { get; set; }
    }
}
