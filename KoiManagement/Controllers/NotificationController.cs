using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoiManagement.Controllers
{
    public class NotificationController : Controller
    {
        // GET: Notification
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Admin()
        {
            ViewBag.userid = "2";
            return View();
        }
        public ActionResult Client()
        {
            return View();
        }
    }
}