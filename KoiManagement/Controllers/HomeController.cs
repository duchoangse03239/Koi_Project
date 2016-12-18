using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Models;

namespace KoiManagement.Controllers
{
    public class HomeController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.home = db.Articles.Where(p => p.TypeID == 3).FirstOrDefault();
            return View();
        }
    }
}