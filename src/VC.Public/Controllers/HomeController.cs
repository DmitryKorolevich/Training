using Microsoft.AspNetCore.Mvc;
using VitalChoice.Core.Base;

namespace VC.Public.Controllers
{
    public class HomeController : BaseMvcController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}