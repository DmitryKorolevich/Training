using System;
using VitalChoice.Core.Infrastructure.Models;

namespace VitalChoice.Models.Setting
{
    public class LogItemListFilter : FilterModelBase
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public string LogLevel { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }
    }
}