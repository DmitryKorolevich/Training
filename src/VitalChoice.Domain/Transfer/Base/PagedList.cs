using System.Collections.Generic;

namespace VitalChoice.Domain.Transfer.Base
{
    public class PagedList<T> where T : class 
    {
        public IEnumerable<T> Items { get; set; }
        public int Count { get; set; }

        public PagedList(IEnumerable<T> items, int count)
        {
            Count = count;
            Items = items;
        }

	    public PagedList()
	    {

	    }
    }
}