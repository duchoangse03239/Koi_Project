using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Models;

namespace KoiManagement.Controllers
{
    public class KoiFarmController : Controller
    {
        // GET: KoiFarm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CreateKoiFarm()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CreateKoiFarm(HtmlContent Content)
        {
            return View();
        }
    }
}