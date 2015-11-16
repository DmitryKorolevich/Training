using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderOptionValue : OptionValue<OrderOptionType>
    {
        public int IdOrder { get; set; }
    }
}
