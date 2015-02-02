using System.Collections.Generic;

namespace VitalChoice.Validation.Models
{
    public class PageList<T>
    {
        public PageList()
        {
            Items = new List<T>();
        }

        public PageList(List<T> data, int allCount)
        {
            Items = data;
            AllCount = allCount;
        }

        public PageList(IEnumerable<T> data, int allCount)
        {
            Items = new List<T>();
            Items.AddRange(data);
            AllCount = allCount;
        }

        public List<T> Items { get; set; }

        public int AllCount { get; set; }
    }
}