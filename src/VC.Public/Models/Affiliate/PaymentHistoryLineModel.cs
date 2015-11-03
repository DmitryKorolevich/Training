using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VC.Public.Models.Affiliate
{
    public class PaymentHistoryLineModel
    {
	    public DateTime DateCreated { get; set; }

	    public decimal Amount { get; set; }
    }
}
