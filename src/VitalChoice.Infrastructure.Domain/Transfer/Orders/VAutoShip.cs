using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class VAutoShip : Entity
    {
	    public int AutoShipFrequency { get; set; }

	    public DateTime? LastAutoShipDate { get; set; }
    }
}