using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserBusinessLayer;
using UserBusinessLayer.ViewModels;

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
        [ActionName("Login")]
        public ActionResult Login_get(string flash)
        {
            ViewBag.Title = "EventHub - Login";
            ViewBag.Flash = flash;

            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult Login_post(string flash, FormCollection formCollection)
        {
            ViewBag.Title = "EventHub - Login";
            ViewBag.Flash = flash;

            LoginViewModel currentUser = new LoginViewModel();
            currentUser.UsernameEmail = formCollection["UsernameEmail"];
            currentUser.Passwd = formCollection["Passwd"];
            currentUser.RememberMe = Convert.ToBoolean(formCollection["RememberMe"].Split(',')[0]);

            if(ModelState.IsValid)
            {

            }

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

            RegisterViewModel newUser = new RegisterViewModel();
            newUser.UserName = formCollection["UserName"];
            newUser.Email = formCollection["Email"];
            newUser.ConfirmEmail = formCollection["ConfirmEmail"];
            newUser.Passwd = formCollection["Passwd"];
            newUser.ConfirmPasswd = formCollection["ConfirmPasswd"];

            if (ModelState.IsValid)
            {
                UserBusinessLogic userBusinessLogic = new UserBusinessLogic();

                if (!userBusinessLogic.addNewUser(newUser, ModelState))
                {
                    return View(newUser);
                }
                else
                {
                    return RedirectToAction("Login", new { flash = "Joined" });
                }
                
            }
            return View(newUser);
        }
    }
}