using Microsoft.AspNet.Mvc;
using VitalChoice.Core.Base;

namespace VC.Public.Controllers
{
    public class HomeController : BaseMvcController
    {
        public IActionResult Index()
        {
            return View();
        }

		public IActionResult Category()
		{
			return View();
		}

		public IActionResult Category1()
		{
			return View();
		}
	}
}