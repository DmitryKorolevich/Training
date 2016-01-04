using Microsoft.AspNet.Http;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services
{
	public interface IRedirectService
	{
		Task<bool> CheckRedirects(HttpContext context);
	}
}
