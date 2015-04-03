using System;

namespace VitalChoice.Core.Infrastructure.Models
{
    public class SortFilter
    {
		public string Path { get; set; }

		public SortOrder? SortOrder { get; set; }
	}
}