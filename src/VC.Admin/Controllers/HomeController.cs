using Microsoft.AspNet.Mvc;

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
    }
}