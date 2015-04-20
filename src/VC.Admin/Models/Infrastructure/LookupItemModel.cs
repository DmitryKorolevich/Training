using System;

namespace VitalChoice.Models.Infrastructure
{
    public class LookupItemModel<T>
    {
	    public T Key { get; set; }

	    public string Text { get; set; }
    }
}