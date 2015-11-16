using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Log
{
    public class CommonLogQuery : QueryObject<CommonLogItem> {
        public CommonLogQuery GetItems(string logLevel = null, string message = null, string source=null, DateTime? from = null, DateTime? to =null)
        {
            if (from.HasValue)
            {
                Add(x => x.Date >= from.Value);
            }
            if (to.HasValue)
            {
                Add(x => x.Date <= to.Value);
            }
            if (!String.IsNullOrEmpty(logLevel))
            {
                Add(x => x.LogLevel == logLevel);
            }
            if (!String.IsNullOrEmpty(message))
            {
                Add(x => x.ShortMessage.Contains(message));
            }
            if (!String.IsNullOrEmpty(source))
            {
                Add(x => x.Source.Contains(source));
            }
            return this;
        }
    }
}