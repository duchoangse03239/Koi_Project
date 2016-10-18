using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoiManagement.Controllers
{
    public class AccountController : Controller
    {
  
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            Session.Add("User", "1");
            return View();
        }

        public ActionResult Logout()
        {
            return View();
        }

	}
}