using System;
using VitalChoice.Domain.Entities.Logs;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VitalChoice.Models.Setting
{
    public class LogListItemModel : Model<CommonLogItem, IMode>
    {
        public DateTime Date { get; set; }

        public string LogLevel { get; set; }

        public string Source { get; set; }

        public string ShortMessage { get; set; }

        public string Message { get; set; }

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