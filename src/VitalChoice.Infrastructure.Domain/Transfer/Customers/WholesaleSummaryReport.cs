using System;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Infrastructure.Domain.Transfer.Customers
{
    public class WholesaleSummaryReport
    {
        public int AllAccounts { get; set; }

        public int ActiveAccounts { get; set; }    

        public int NewActiveAccounts { get; set; }

        public ICollection<WholesaleSummaryReportTradeClassItem> TradeClasses { get; set; }
    }
}
