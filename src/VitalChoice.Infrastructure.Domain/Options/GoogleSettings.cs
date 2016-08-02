using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Options
{
    public class GoogleSettings
    {
	    public string ClientId { get; set; }
        public string KeyFile { get; set; }
        public string KeyPass { get; set; }
        public string GAAccountId { get; set; }
    }
}
