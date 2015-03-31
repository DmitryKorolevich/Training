using System;
using System.Collections.Generic;

namespace VitalChoice.Domain.Entities.Base
{
    public class PagedList<T> where T : Entity
    {
        public IEnumerable<T> Items { get; set; }
        public int Count { get; set; }

        public PagedList(IEnumerable<T> items, int count)
        {
            Count = count;
            Items = items;
        }
    }
}