using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services
{
	public interface IRedirectViewService
	{
		Task<bool> CheckRedirects(HttpContext context);
    }
}
