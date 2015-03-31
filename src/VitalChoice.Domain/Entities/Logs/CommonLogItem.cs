using System;

namespace VitalChoice.Domain.Entities.Logs
{
	public class CommonLogItem : Entity
	{
		public DateTime Date { get; set; }

		public string LogLevel { get; set; }

		public string Source { get; set; }

		public string Message { get; set; }
	}
}
