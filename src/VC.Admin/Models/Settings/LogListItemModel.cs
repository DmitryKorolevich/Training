using System;
using VitalChoice.Ecommerce.Domain.Entities.Logs;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Setting
{
    public class LogListItemModel : BaseModel
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