using Microsoft.AspNetCore.Mvc;

namespace VC.Admin.Controllers
{
    public class HomeController : Controller
    {
	    public HomeController()
	    {
	    }

	    public IActionResult Index()
        {
            return View();
        }

        public IActionResult IE()
        {
            return View();
        }
    }
}