using System;

namespace VitalChoice.Infrastructure.Domain.Avatax
{
    [Flags]
    public enum TaxGetType
    {
        Commit = 1,
        PerishableOnly = 2,
        NonPerishableOnly = 4,
        UseBoth = PerishableOnly | NonPerishableOnly,
        SavePermanent = 8
    }
}
