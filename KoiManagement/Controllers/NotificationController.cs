using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;

namespace KoiManagement.Controllers
{
    public class NotificationController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();
        NotificationDAO notificationDao = new NotificationDAO();
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
        public ActionResult ListNotification()
        {
            int memberId=0;
            if (Session[SessionAccount.SessionUserId]!=null)
            {
                memberId = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            }
            var list = notificationDao.GetListNotifications(memberId);
            return View(list);
        }
    }
}