using System;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VC.Admin.Models.Setting
{
    public class LogItemListFilter : FilterBase
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public string LogLevel { get; set; }

        public string Source { get; set; }

        public string Message { get; set; }
    }
}