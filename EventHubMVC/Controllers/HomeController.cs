using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserBusinessLayer;
using UserBusinessLayer.ViewModels;
using EventBusinessLayer;
using EventBusinessLayer.ViewModels;

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
                UserBusinessLogic userBusinessLogic = new UserBusinessLogic();

                if (!userBusinessLogic.loginUser(currentUser, ModelState))
                {
                    return View(currentUser);
                }
                else
                {
                    if(currentUser.UsernameEmail.Contains("@"))
                    {
                        Session["Email"] = currentUser.UsernameEmail;
                        Session["Username"] = false;
                    }
                    else
                    {
                        Session["Username"] = currentUser.UsernameEmail;
                        Session["Email"] = false;
                    }
                    
                    return RedirectToAction("Index");
                }
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


        [HttpGet]
        public ActionResult Logout()
        {
            ViewBag.Title = "EventHub - Logout";

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", new { flash = "LoggedOut" });
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Title = "EventHub - Create new event";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<SelectListItem> categories = eventBusinessLogic.getCatergoryList();

            ViewBag.Catergories = new SelectList(categories, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Create(EventViewModel newEvent)
        {
            ViewBag.Title = "EventHub - Create new event";
            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();

            if (ModelState.IsValid)
            {

                string sessionTag;

                if(Session["Username"] != null)
                {
                    sessionTag =Session["Username"].ToString();
                }
                else
                {
                    sessionTag = Session["Email"].ToString();
                }

               
                eventBusinessLogic.addNewEvent(newEvent, ModelState, sessionTag);

                return RedirectToAction("Index");
            }


           List<SelectListItem> categories = eventBusinessLogic.getCatergoryList();

           ViewBag.Catergories = new SelectList(categories, "Value", "Text");

           return View(newEvent);
        }
    }
}