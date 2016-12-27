using System;
using Microsoft.AspNetCore.Http;

namespace VitalChoice.Interfaces.Services
{
	public interface IRedirectViewService : IDisposable
	{
		bool CheckRedirects(HttpContext context);
    }
}
