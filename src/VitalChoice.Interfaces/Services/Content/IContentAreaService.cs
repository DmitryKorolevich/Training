using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Interfaces.Services.Content
{
    public interface IContentAreaService
    {
	    Task<IList<ContentArea>> GetContentAreasAsync();

	    Task<ContentArea> UpdateContentAreaAsync(ContentArea contentArea);

	    Task<ContentArea> GetContentAreaAsync(int id);

	    Task<ContentArea> GetContentAreaByNameAsync(string name);
    }
}
