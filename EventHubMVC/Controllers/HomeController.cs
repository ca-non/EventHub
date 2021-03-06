﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using UserBusinessLayer;
using UserBusinessLayer.ViewModels;
using EventBusinessLayer;
using EventBusinessLayer.ViewModels;
using EventHubMVC.Models;

namespace EventHubMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "EventHub";
            
            return View();
        }

        public ActionResult SearchEvents(string searchBox)
        {
            ViewBag.Title = "Search events";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventInfo> eventInfoList = eventBusinessLogic.getSearchResults(searchBox);

            return View(eventInfoList);
        }

        public ActionResult BrowseEvents()
        {
            ViewBag.RunScript = true;
            ViewBag.Title = "Browse events";

            return View();
        }

        public PartialViewResult BrowseEvent(string browseType)
        {
            ViewBag.RunScript = false;
            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventInfo> eventInfoList = eventBusinessLogic.getBrowseResults(browseType);

            return PartialView("~/Views/Home/PartialViews/_BrowseEvents.cshtml", eventInfoList);
        }

        [HttpPost]
        public ActionResult BrowseEvents(FormCollection formCollection)
        {
            ViewBag.Title = "Browse events";
            ViewBag.RunScript = false;

            EventDT eventDT = new EventDT();
            eventDT.FromTime = formCollection["from-time"];
            eventDT.ToTime = formCollection["to-time"];
            eventDT.FromDate = formCollection["from-date"];
            eventDT.ToDate = formCollection["to-date"];
            
            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventInfo> eventInfoList = eventBusinessLogic.getDTResults(eventDT);

           if(eventInfoList.Count == 0)
            {
                ViewBag.Results = "NoResults";
            }
            else
            {
                ViewBag.Results = "Results";
            }

            return View(eventInfoList);
        }
        
        public ActionResult Event(int Id)
        {
            ViewBag.Tile = "Event";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            EventInfo eventInfo = eventBusinessLogic.getEvent(Id);

            return View(eventInfo);
        }


        public PartialViewResult All()
        {
            ViewBag.ImageName = "All.jpg";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventCard> eventCards = eventBusinessLogic.getAllEvents();

            return PartialView("~/Views/Home/PartialViews/_HomeEvents.cshtml", eventCards);
        }

        public PartialViewResult Festivals()
        {
            ViewBag.ImageName = "Festivals.jpg";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventCard> eventCards = eventBusinessLogic.getAllEvents("Festivals");

            return PartialView("~/Views/Home/PartialViews/_HomeEvents.cshtml", eventCards);
        }

        public PartialViewResult FoodAndDrink()
        {
            ViewBag.ImageName = "FoodAndDrink.jpg";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventCard> eventCards = eventBusinessLogic.getAllEvents("Food & Drink");

            return PartialView("~/Views/Home/PartialViews/_HomeEvents.cshtml", eventCards);
        }

        public PartialViewResult Art()
        {
            ViewBag.ImageName = "Art.jpg";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventCard> eventCards = eventBusinessLogic.getAllEvents("Art");

            return PartialView("~/Views/Home/PartialViews/_HomeEvents.cshtml", eventCards);
        }

        public PartialViewResult Movies()
        {
            ViewBag.ImageName = "Movies.jpg";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventCard> eventCards = eventBusinessLogic.getAllEvents("Movies");

            return PartialView("~/Views/Home/PartialViews/_HomeEvents.cshtml", eventCards);
        }

        public PartialViewResult Gaming()
        {
            ViewBag.ImageName = "Gaming.jpg";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventCard> eventCards = eventBusinessLogic.getAllEvents("Gaming");

            return PartialView("~/Views/Home/PartialViews/_HomeEvents.cshtml", eventCards);
        }

        public PartialViewResult Sports()
        {
            ViewBag.ImageName = "Sports.jpg";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventCard> eventCards = eventBusinessLogic.getAllEvents("Sports");

            return PartialView("~/Views/Home/PartialViews/_HomeEvents.cshtml", eventCards);
        }

        public PartialViewResult Public()
        {
            ViewBag.ImageName = "Public.jpg";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<EventCard> eventCards = eventBusinessLogic.getAllEvents("Public");

            return PartialView("~/Views/Home/PartialViews/_HomeEvents.cshtml", eventCards);
        }

        public ActionResult About()
        {
            ViewBag.Title = "EventHub - About";

            return View();
        }

        [HttpPost]
        public ActionResult AboutFeedback(ContactVM contactVM)
        {
            ViewBag.Title = "EventHub - About";

            if (ModelState.IsValid)
            {
                MainAppLogic mainAppLogic = new MainAppLogic();
                mainAppLogic.addFeedback(contactVM);
            }
            else
            {
                return View("About", contactVM);
            }

            return RedirectToAction("AboutFeedback", new { feedback = true });
        }


        public ActionResult AboutFeedback(bool? feedback)
        {
            ViewBag.Title = "EventHub - About";

            if(feedback != null && feedback == true)
            {
                return View();
            }
            else
            { 
                return RedirectToAction("About");
            }
        }

        [HttpGet]
        [ActionName("Login")]
        public ActionResult Login_get(string flash)
        {
            if (Session["LoggedIn"] != null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Title = "EventHub - Login";
            ViewBag.Flash = flash;

            return View();
        }

        [HttpPost]
        [ActionName("Login")]
        public ActionResult Login_post(string flash, FormCollection formCollection)
        {
            if (Session["LoggedIn"] != null)
            {
                return RedirectToAction("Index");
            }

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
                        Session["LoggedIn"] = "Logged";
                    }
                    else
                    {
                        Session["Username"] = currentUser.UsernameEmail;
                        Session["Email"] = false;
                        Session["LoggedIn"] = "Logged";
                    }
                    
                    return RedirectToAction("Index");
                }
            }

            return View();
        }

        [HttpGet]
        public ActionResult ForgotPassword(string resetFlash)
        {
            ViewBag.Title = "EventHub - Reset Password";
            ViewBag.resetFlash = resetFlash;

            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(ForgotPasswordViewModel resetData)
        {
            ViewBag.Title = "EventHub - Reset Password";

            if(ModelState.IsValid)
            {
                UserBusinessLogic userBusinessLogic = new UserBusinessLogic();
                if(userBusinessLogic.resetPassword(resetData))
                {
                    return RedirectToAction("ForgotPassword", new { resetFlash = "Reset" });
                }
                else
                {
                    return RedirectToAction("ForgotPassword", new { resetFlash = "ResetFalse" });
                } 
            }

            return View();
        }

        public ActionResult ResetPassword(string uid)
        {
            ViewBag.Title = "EventHub - Reset Password";

            UserBusinessLogic userBusinessLogic = new UserBusinessLogic();

            if(userBusinessLogic.checkForResetLinkValid(uid))
            {
                return View();
            }
            else
            {
                return RedirectToAction("LinkExpired", "Home");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordViewModel resetPasswordVM)
        {
            ViewBag.Title = "EventHub - Reset Password";

            if(ModelState.IsValid)
            {
                String GUID = Request.QueryString["uid"];

                UserBusinessLogic userBusinessLogic = new UserBusinessLogic();
                userBusinessLogic.changePassword(resetPasswordVM, GUID);

                return RedirectToAction("Login", new { flash = "PasswordChanged" });
            }

            return View();
        }

        public ActionResult LinkExpired()
        {
            ViewBag.Title = "EventHub - Link Expired";

            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (Session["LoggedIn"] != null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.Title = "EventHub - Register";

            return View();
        }

        
        [HttpPost]
        public ActionResult Register(FormCollection formCollection)
        {
            if (Session["LoggedIn"] != null)
            {
                return RedirectToAction("Index");
            }
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

        public JsonResult IsUsernameAvailable(string UserName)
        {
            UserBusinessLogic userBusinessLogic = new UserBusinessLogic();
            return Json(!userBusinessLogic.checkUsernameRegister(UserName), JsonRequestBehavior.AllowGet);
        }

        public JsonResult IsEmailAvailable(string Email)
        {
            UserBusinessLogic userBusinessLogic = new UserBusinessLogic();
            return Json(!userBusinessLogic.checkEmailRegister(Email), JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult Logout()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("ForbiddenError", "Error");
            }

            ViewBag.Title = "EventHub - Logout";

            Session.Clear();
            Session.Abandon();

            return RedirectToAction("Login", new { flash = "LoggedOut" });
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("ForbiddenError", "Error");
            }

            ViewBag.Title = "EventHub - Create new event";

            EventBusinessLogic eventBusinessLogic = new EventBusinessLogic();
            List<SelectListItem> categories = eventBusinessLogic.getCatergoryList();

            ViewBag.Catergories = new SelectList(categories, "Value", "Text");

            return View();
        }

        [HttpPost]
        public ActionResult Create(EventViewModel newEvent)
        {
            if (Session["LoggedIn"] == null)
            {
                return RedirectToAction("Index");
            }

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