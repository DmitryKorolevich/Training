using System;

namespace VitalChoice.Core.Infrastructure.Models
{
    public abstract class FilterModelBase
    {
		protected FilterModelBase()
		{
			//TotalItemCountRequired = true;
		}

		public string SearchText { get; set; }

	//	public int StartIndex { get; set; }

		public SortFilter[] Sortings { get; set; }

		//public int? PageItemCount
		//{
		//	get
		//	{
		//		return _pageItemCount;
		//	}
		//	set
		//	{
		//		if (value.HasValue && value.Value < 0)
		//			throw new InvalidOperationException();

		//		_pageItemCount = value;
		//	}
		//}
		//private int? _pageItemCount;

		//public bool TotalItemCountRequired { get; set; }

	}
}