using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class TimeZoneHelper
    {
        public static TimeZoneInfo PstTimeZoneInfo { get; } = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
    }
}
