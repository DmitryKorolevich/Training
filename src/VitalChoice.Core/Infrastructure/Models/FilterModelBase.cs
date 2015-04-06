using System;

namespace VitalChoice.Core.Infrastructure.Models
{
    public abstract class FilterModelBase
    {
		protected FilterModelBase()
		{
			
		}

		public string SearchText { get; set; }

        public Paging Paging { get; set; }

        public SortFilter[] Sortings { get; set; }
	}
}