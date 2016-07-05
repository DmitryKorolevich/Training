namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public enum ExportSide
    {
        All = 0,
        Perishable = 1,
        NonPerishable = 2
    }

    public enum POrderType
    {
        All = 0,
        P = 1,
        NP = 2,
        PNP = 3,
        Other = 4,
    }
}