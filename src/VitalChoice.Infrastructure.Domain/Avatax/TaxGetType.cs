using System;

namespace VitalChoice.Infrastructure.Domain.Avatax
{
    [Flags]
    public enum TaxGetType
    {
        Commit = 1,
        Perishable = 2,
        NonPerishable = 4,
        UseBoth = Perishable | NonPerishable,
        SavePermanent = 8
    }
}
