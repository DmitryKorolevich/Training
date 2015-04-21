using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer
{
    public class SortFilter
    {
		public string Path { get; set; }

		public SortOrder SortOrder { get; set; }
	}
}