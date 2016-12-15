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
            int memberId=12;
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
        [HttpPost]
        public ActionResult IsReadNo(string NoID)
        {
            Notification No = db.Notifications.Find(int.Parse(NoID));
            No.isRead = true;
            db.Notifications.Attach(No);
            db.Entry(No).Property(x => x.isRead).IsModified = true;
            int result = db.SaveChanges();
            //return View();
            if (result == 1)
            {
                return Json(new { result = true });
            }
            else
            {
                return Json(new
                {
                    result = false
                });
            }
        }
        [HttpPost]
        public ActionResult IsReadMe(string Meid)
        {
            Models.Message Me = db.Messages.Find(int.Parse(Meid));
            Me.IsRead = true;
            db.Messages.Attach(Me);
            db.Entry(Me).Property(x => x.IsRead).IsModified = true;
            int result = db.SaveChanges();
            //return View();
            if (result == 1)
            {
                return Json(new { result = true });
            }
            else
            {
                return Json(new
                {
                    result = false
                });
            }
        }
        


        //public JsonResult getDetailMessage(int messageid)
        //{
        //    int memberId = 0;
        //    if (Session[SessionAccount.SessionUserId] == null)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }

        //    memberId = int.Parse(Session[SessionAccount.SessionUserId].ToString());
        //    StatusObjForJsonResult obj = new StatusObjForJsonResult();
        //    ViewBag.meDetail = messageDao.GetListMeDetail(messageid);
        //    return Json(obj);
        //}


    }
}