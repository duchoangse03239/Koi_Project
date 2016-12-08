using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.DAL;

namespace KoiManagement.Controllers
{
    public class MessageController : Controller
    {
        // GET: Message
        public ActionResult Index()
        {
            return View();
        }
        MessageDAO messageDao = new MessageDAO();
        public ActionResult ListMessage1(int messageid)
        {

            // id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //if (id == null && Session[SessionAccount.SessionUserId] != null)
            //{
            //    id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //}
            //KoiDAO koiDao = new KoiDAO();
            //MemberDAO mDAO = new MemberDAO();
            //KoiFarmDAO koiFarmDao = new KoiFarmDAO();
            //ViewBag.Member = mDAO.GetMemberbyID(id);
            //ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            //ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            int memberId = 12;
            MessageDAO messageDao = new MessageDAO();
           // memberId = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            ViewBag.meDetail = messageDao.GetListMeDetail(messageid);
            return View("~/Views/Message/ListMessage.cshtml");
        }

        [ChildActionOnly]
        public ActionResult ListMessage()
        {

            // id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //if (id == null && Session[SessionAccount.SessionUserId] != null)
            //{
            //    id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //}
            //KoiDAO koiDao = new KoiDAO();
            //MemberDAO mDAO = new MemberDAO();
            //KoiFarmDAO koiFarmDao = new KoiFarmDAO();
            //ViewBag.Member = mDAO.GetMemberbyID(id);
            //ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            //ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            int memberId = 12;
          
           // memberId = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            ViewBag.meDetail = messageDao.GetListMeDetail(memberId);
            return View("~/Views/Message/ListMessage.cshtml");
        }

        public ActionResult MessageDetail(int id)
        {
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }



            int memberId = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            MessageDAO messageDao = new MessageDAO();
            // memberId = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            ViewBag.meDetail = messageDao.GetListMeDetail(id);
            //if (ViewBag.meDetail.Count == 0)
            //{
            //    RedirectToAction("PageNotFound", "Error");
            //}
            ViewBag.messageID = id;
            ViewBag.Ownerid = messageDao.getOwner(id);
            return View();
        }

         public JsonResult AddMessage(int messageid, string content,int tomember)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            var ownerid = messageDao.getOwner(messageid);
            try
            {

                if (Session[SessionAccount.SessionUserId] == null)
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy đăng nhập.";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(content))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập nội dung.";
                    return Json(obj);
                }
                int UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());
                Models.Message me;
                if (UserID == ownerid.MemberID)
                {
                    me = new Models.Message(content, DateTime.Now, UserID, null, content, messageid, false, true);
                }
                else
                {
                    me = new Models.Message(content, DateTime.Now, null, UserID, content, messageid, false, true);
                }
                 if (messageDao.AddMessage(me))
                {
                    obj.Status = 1;

                    // ownerDao.ChangeOwner(username, int.Parse(koiId))
                    return Json(obj);
                }

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