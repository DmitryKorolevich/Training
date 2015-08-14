using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Order;
using VitalChoice.Domain.Transfer.Products;

namespace VitalChoice.Interfaces.Services.Order
{
	public interface IOrderService
    {
        Task<ICollection<VOrder>> GetSkusAsync(VOrderFilter filter);
    }
}
