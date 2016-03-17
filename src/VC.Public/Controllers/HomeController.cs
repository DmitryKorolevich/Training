using Microsoft.AspNet.Mvc;
using VitalChoice.Core.Base;
using VitalChoice.Core.Services;

namespace VC.Public.Controllers
{
    public class HomeController : BaseMvcController
    {
        public IActionResult Index()
        {
            return View();
        }

        public HomeController(IPageResultService pageResultService) : base(pageResultService)
        {
        }
    }
}