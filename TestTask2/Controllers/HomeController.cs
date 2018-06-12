using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestTask2.AgilityPackClasses;
using TestTask2.EF;

namespace TestTask2.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            return View();
        }

        public ActionResult Product(int? id)
        {
            if (id == null) RedirectToAction("Index");
            return View();
        }
    }
}
