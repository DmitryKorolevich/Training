using System;

namespace VitalChoice.Ecommerce.Domain.Entities.Logs
{
	public class CommonLogItem : Entity
	{
		public DateTime Date { get; set; }

		public string LogLevel { get; set; }

		public string Source { get; set; }

        public string ShortMessage { get; set; }

        public string Message { get; set; }
	}
}
