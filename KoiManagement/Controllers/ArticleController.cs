using KoiManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoiManagement.Controllers
{
    public class ArticleController : Controller
    {
        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login
        public ActionResult AddArticle(string returnUrl)
        {
            //if (Session[SessionAccount.SessionUserId] != null)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            //ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddArticle(string username, string password)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            try
            {
               
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }

    }
}