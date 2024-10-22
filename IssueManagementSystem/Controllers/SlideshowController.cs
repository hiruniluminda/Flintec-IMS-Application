using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IssueManagementSystem.Controllers
{
    public class SlideshowController : Controller
    {
        // GET: Slideshow
        public ActionResult Index()
        {
            return View();
        }
    }
}