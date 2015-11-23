using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VitalChoice.Core.Infrastructure.Helpers.ReCaptcha
{
    public class ReCaptcha
    {
		[JsonProperty("success")]
		public string Success { get; set; }

		[JsonProperty("error-codes")]
		public List<string> ErrorCodes { get; set; }

	}
}
