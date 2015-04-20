namespace VitalChoice.Domain.Entities.Base
{
    public class LookupItem<T>
    {
	    public T Key { get; set; }

	    public string Text { get; set; }
    }
}