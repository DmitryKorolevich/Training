using VitalChoice.Core.Infrastructure.Models;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Models.ContentManagement
{
    public class MasterContentItemListFilter : FilterModelBase
    {
	    public ContentType? Type { get; set; }
    }
}