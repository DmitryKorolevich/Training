using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Orders
{
    public class OrderOptionValue : OptionValue<OrderOptionType>
    {
        public int IdOrder { get; set; }
    }
}
