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
        MessageDAO messageDao = new MessageDAO();
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
                if (Session[SessionAccount.SessionUserId] == null)
                {
                    return RedirectToAction("Login", "Account");
                }
                memberId = int.Parse(Session[SessionAccount.SessionUserId].ToString());

            ViewBag.ListNoCF = notificationDao.GetListNoCF(memberId).ToList();
            ViewBag.ListNo = notificationDao.GetListNo(memberId).ToList();
            ViewBag.listMe = messageDao.GetListMessage(memberId);
            MemberDAO MDao = new MemberDAO();
            KoiDAO koiDao = new KoiDAO();
            MemberDAO mDAO = new MemberDAO();
            KoiFarmDAO koiFarmDao = new KoiFarmDAO();
            ViewBag.Member = mDAO.GetMemberbyID(memberId);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(memberId);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(memberId);

            return View();
        }


    }
}