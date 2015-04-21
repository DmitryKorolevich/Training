using VitalChoice.Domain.Constants;

namespace VitalChoice.Domain.Transfer.Base
{
    public class Paging
    {
	    public Paging()
	    {
		    PageIndex = 1;
		    PageItemCount = BaseAppConstants.DEFAULT_LIST_TAKE_COUNT;
	    }

	    public int PageIndex { get; set; }
        public int PageItemCount { get; set; }
    }
}