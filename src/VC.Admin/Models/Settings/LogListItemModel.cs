using System;
using System.Globalization;
using VitalChoice.Ecommerce.Domain.Entities.Logs;
using VitalChoice.Infrastructure.Azure;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Settings
{
    public class LogListItemModel : BaseModel
    {
        public DateTime Date { get; set; }

        public string LogLevel { get; set; }

        public string Source { get; set; }

        public string ShortMessage { get; set; }

        public string Message { get; set; }

        public LogListItemModel(TableLogEntity logEntity)
        {
            if (logEntity != null)
            {
                Date = DateTime.ParseExact(logEntity.RowKey, "yyyy-MM-ddTHH:mm:ss.fffffff", CultureInfo.InvariantCulture);
                LogLevel = logEntity.PartitionKey;
                Source = logEntity.Source;
                ShortMessage = logEntity.ShortMessage;
                Message = logEntity.Message;
            }
        }

        public LogListItemModel(CommonLogItem item)
        {
            if(item!=null)
            {
                Date = item.Date;
                LogLevel = item.LogLevel;
                Source = item.Source;
                ShortMessage = item.ShortMessage;
                Message = item.Message;
            }
        }
    }
}