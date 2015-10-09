using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Avatax
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
