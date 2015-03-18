using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Mvc;
using VitalChoice.Business.Services.Contracts;

namespace VitalChoice.Controllers
{
    public class HomeController : Controller
    {
	    //public HomeController(ICommentService commentService)
	    public HomeController()
	    {

	    }

	    public IActionResult Index()
        {
            return View();
        }
    }
}