using VitalChoice.Ecommerce.Domain.Options;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class JobSettings: AppOptionsBase
	{
	    public string AutoShipSchedule { get; set; }

		public string DefaultCultureId { get; set; }
	}
}
