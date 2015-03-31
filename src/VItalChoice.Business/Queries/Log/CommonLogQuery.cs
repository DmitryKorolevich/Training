using System;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Logs;

namespace VitalChoice.Business.Queries.Log
{
    public class CommonLogQuery : QueryObject<CommonLogItem> {
        public CommonLogQuery GetItems(string logLevel = null, string message = null, DateTime? from = null, DateTime? to =null)
        {
            if (!String.IsNullOrEmpty(logLevel))
            {
                Add(x => x.LogLevel.Equals(logLevel));
            }
            if (!String.IsNullOrEmpty(message))
            {
                Add(x => x.Message.Contains(message));
            }
            if(from.HasValue)
            {
                Add(x => x.Date >= from.Value);
            }
            if (to.HasValue)
            {
                Add(x => x.Date <= to.Value);
            }
            return this;
        }
    }
}