using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class PaymentValidationExpressions
    {
        public static readonly Regex MasterCardRegex = new Regex("^5[1-5][0-9]{14}$", RegexOptions.Compiled);
        public static readonly Regex VisaRegex = new Regex("^4[0-9]{12,15}$", RegexOptions.Compiled);
        public static readonly Regex DiscoverRegex = new Regex("^6(011[0-9]{4})|(5[0-9]{6})|(22[1-9]{5})[0-9]{8}$", RegexOptions.Compiled);
        public static readonly Regex AmericanExpressRegex = new Regex("^3(4|7)[0-9]{13}$", RegexOptions.Compiled);
    }
}
