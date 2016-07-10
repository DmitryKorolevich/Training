using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Content;

namespace VitalChoice.Interfaces.Services.Content
{
    public interface IContentAreaService
    {
	    Task<IList<ContentArea>> GetContentAreasAsync();

	    Task UpdateContentAreaAsync(ContentArea contentArea);

	    Task<ContentArea> GetContentAreaAsync(int id);

	    Task<ContentArea> GetContentAreaByNameAsync(string name);
    }
}
