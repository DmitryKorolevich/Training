using Microsoft.AspNetCore.Http;

namespace VitalChoice.Interfaces.Services
{
	public interface IRedirectViewService
	{
		bool CheckRedirects(HttpContext context);
    }
}
