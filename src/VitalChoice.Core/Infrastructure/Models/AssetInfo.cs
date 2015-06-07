using System.Collections.Generic;

namespace VitalChoice.Core.Infrastructure.Models
{
	public class AssetInfo
	{
		public string MinifiedFileName { get; set; }
		public IEnumerable<string> Files { get; set; }
	}
}