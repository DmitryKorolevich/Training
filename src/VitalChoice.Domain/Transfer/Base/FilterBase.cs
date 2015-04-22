namespace VitalChoice.Domain.Transfer.Base
{
    public class FilterBase
    {
		public FilterBase()
		{
			Paging = new Paging();
			Sorting = new SortFilter();
        }

		public string SearchText { get; set; }

        public Paging Paging { get; set; }

        public SortFilter Sorting { get; set; }
	}
}