namespace VitalChoice.Domain.Transfer.Base
{
    public struct LookupItem<T>
    {
	    public T Key { get; set; }
	    public string Text { get; set; }
    }
}