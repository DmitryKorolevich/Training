namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public enum OrderType
    {
        Normal = 1,
        AutoShip = 2,
        DropShip = 3,
        GiftList = 4,
        Reship = 5,
        Refund = 6
    }
}