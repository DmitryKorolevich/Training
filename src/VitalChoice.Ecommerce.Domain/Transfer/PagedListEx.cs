using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Transfer
{
    public class PagedListEx<T>: PagedList<T>
    {
		public int Index { get; set; }

	    public PagedListEx() : base()
	    {
	    }

	    public PagedListEx(IList<T> items, int count, int index) : base(items, count)
	    {
		    Index = index;
	    }
    }
}