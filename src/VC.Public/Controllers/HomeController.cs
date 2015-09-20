using Microsoft.AspNet.Mvc;

namespace VC.Public.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

		public IActionResult Category()
		{
			return View();
		}
	}
}