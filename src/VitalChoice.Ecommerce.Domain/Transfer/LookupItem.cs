namespace VitalChoice.Ecommerce.Domain.Transfer
{
    public class LookupItem<T>
    {
	    public T Key { get; set; }
	    public string Text { get; set; }
        public bool Hidden { get; set; }
    }
}