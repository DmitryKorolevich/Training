using System.Collections.Generic;

namespace VitalChoice.Domain.Transfer.Base
{
    public struct PagedList<T>
    {
        public IList<T> Items { get; set; }
        public int Count { get; set; }

        public PagedList(IList<T> items, int count)
        {
            Count = count;
            Items = items;
        }
    }
}