using VitalChoice.Infrastructure.Domain.Constants;

namespace VitalChoice.Infrastructure.Domain.Transfer
{
    public class Paging
    {
	    public Paging()
	    {
		    PageIndex = 1;
		    PageItemCount =  BaseAppConstants.DEFAULT_LIST_TAKE_COUNT;
	    }

        public int PageIndex { get; set; }
        public int PageItemCount { get; set; }
    }
}