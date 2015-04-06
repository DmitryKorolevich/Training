using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Models.ContentManagement
{
    public class CategoryTreeFilter : FilterModelBase
    {
	    public ContentType Type { get; set; }
        public RecordStatusCode? Status { get; set; }
    }
}