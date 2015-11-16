namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public enum OrderStatus
    {
        Incomplete = 1,
        Processed = 2,
        Shipped = 3,
        Cancelled = 4,
        Exported = 5,
        ShipDelayed = 6,
        OnHold = 7
    }
}