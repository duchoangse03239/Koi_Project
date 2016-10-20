using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;

namespace KoiManagement.Controllers
{
    public class ErrorController : Controller
    {
        // GET: Error
        public ActionResult Index()
        {
            return View();
        }
        //
        // GET: /Error
        public ActionResult Error404()
        {
          //  return RedirectToAction(ConstantsForCommon.ErrorParam.SystemErrors);
            return View();
        }

        public ActionResult Error500()
        {
           // return RedirectToAction(ConstantsForCommon.ErrorParam.SystemErrors);
            return View();
        }

        public ActionResult SystemError()
        {
            if (!String.IsNullOrEmpty(SessionAccount.SessionName))
            {
                ViewBag.BackLinkTitle = Common.Constants.BackLinkTitleToHome;
                ViewBag.BackUrl = Url.Action("Home", "Home");
            }
            else
            {
                ViewBag.BackLinkTitle =  Common.Constants.BackLinkTitleToLogin;
                ViewBag.BackUrl = Url.Action("login", "login");
            }
            ViewBag.ShowHeader = false;
            ViewBag.ErrorMessage = Common.Message.Loi;
            return View();
        }

    }
}