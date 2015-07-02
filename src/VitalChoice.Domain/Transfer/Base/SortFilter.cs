namespace VitalChoice.Domain.Transfer.Base
{
    public struct SortFilter
    {
		public string Path { get; set; }

		public SortOrder SortOrder { get; set; }
	}
}