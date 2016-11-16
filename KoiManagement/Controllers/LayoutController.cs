using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoiManagement.Controllers
{
    public class LayoutController : Controller
    {
        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult Header()
        {
           // var model = "duc";
            //return View();
            ViewBag.NotiCount = 9;
        return View("~/Views/Shared/_Header.cshtml");
        }
    }
}