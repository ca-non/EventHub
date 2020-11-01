using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserLayer;

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

        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.Title = "EventHub - Login";

            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.Title = "EventHub - Register";

            return View();
        }

        
        [HttpPost]
        public ActionResult Register(FormCollection formCollection)
        {
            ViewBag.Title = "EventHub - Register";

            if (ModelState.IsValid)
            {
                
                User newUser = new User();
                newUser.UserName = formCollection["username"];
                newUser.Email = formCollection["email"];
                newUser.Passwd = formCollection["password"];

                UserBusinessLayer userBusinessLayer = new UserBusinessLayer();

                if (!userBusinessLayer.addNewUser(newUser, ModelState))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Register");
                }
                
            }
            return View();
        }
    }
}