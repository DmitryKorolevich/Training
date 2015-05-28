using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.ContentManagement
{
    public class CategoryTreeFilter : FilterBase
    {
	    public ContentType Type { get; set; }
        public RecordStatusCode? Status { get; set; }
    }
}