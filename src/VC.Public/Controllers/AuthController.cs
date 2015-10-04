using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Public.Models.Auth;

namespace VC.Public.Controllers
{
	[AllowAnonymous]
    public class AuthController : Controller
    {
		[HttpGet]
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

	    [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginModel model, string returnUrl)
	    {
		    return View(model);
	    }
    }
}