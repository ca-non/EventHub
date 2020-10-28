using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EventHubMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "EventHub";
            
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Title = "EventHub - About";

            return View();
        }

        public ActionResult Login()
        {
            ViewBag.Title = "EventHub - Login";

            return View();
        }

        public ActionResult Register()
        {
            ViewBag.Title = "EventHub - Register";

            return View();
        }
    }
}