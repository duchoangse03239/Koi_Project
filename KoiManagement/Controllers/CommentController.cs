using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;

namespace KoiManagement.Controllers
{
    public class CommentController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();
        CommentDAO commentDao= new CommentDAO();
        MemberDAO memberDao = new MemberDAO();
        KoiDAO koiDao = new KoiDAO();
        OwnerDAO ownerDao = new OwnerDAO();
        NotificationDAO notificationDao = new NotificationDAO();
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }
        public bool addNotification(int memberid,int koiID)
        {
            var koi = koiDao.getKoiById(koiID);
            var commenter = memberDao.GetMemberbyID(memberid);
            var Owner = ownerDao.GetOwner(koiID);
            Notification no = new Notification(Owner.MemberID,memberid,1,koiID,DateTime.Now, commenter.Name+ " đã bình luận về "+ koi.KoiName+ " của bạn.", "/Koi/Details/" + koiID, false,true);
            notificationDao.AddNotification(no);

            return true;
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult AddCommentKoi([FromBody] string Comment, int koiID ,decimal RateNum)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            try
            {
                if (Session[SessionAccount.SessionUserId] == null)
                {
                    obj.Status = 3;
                    obj.Message = "Xin hãy đăng nhập để đánh giá.";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(Comment))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập đánh giá > 10 kí tự. ";
                    return Json(obj);
                }
                int UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());

                if (!commentDao.CheckRatingKoi(UserID, koiID))
                {
                    obj.Status = 3;
                    obj.Message = "Bạn đã đánh giá cá Koi này rồi.";
                    return Json(obj);
                }

                Member mem = memberDao.GetMemberbyID(UserID);
                Comment com = new Comment(UserID, koiID, DateTime.Now,null, RateNum, Comment,null,true);
                UserComment userComment = new UserComment(UserID,mem.Name,mem.Image,DateTime.Now, RateNum,Comment);
                ViewBag.UsercommentImage = mem.Image;
                if (commentDao.addComment(com))
                {
                    addNotification(UserID, koiID);
                    obj.Status = 1;
                    //obj.Message = "Bạn đã đổi sang chủ " + username + " thành công vui lòng chờ xác nhận";
                    //obj.JsonObject = NewMemberID.MemberID;
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

        [System.Web.Mvc.HttpPost]
        public JsonResult EditComment( int commentId, string comment)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 3;
                obj.Message = "Xin hãy đăng nhập sửa bình luận.";
                return Json(obj);
            }
            int UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            try
            {
                if (string.IsNullOrWhiteSpace(comment))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập đánh giá > 10 kí tự. ";
                    return Json(obj);
                }
                if (commentDao.editComment(commentId, comment))
                {
                    obj.Status = 1;
                    //obj.Message = "Bạn đã đổi sang chủ " + username + " thành công vui lòng chờ xác nhận";
                    //obj.JsonObject = NewMemberID.MemberID;
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

        [System.Web.Mvc.HttpPost]
        public JsonResult AddSonCommentKoi( int commentId,string comment, int koiID)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 3;
                obj.Message = "Xin hãy đăng nhập để bình luận.";
                return Json(obj);
            }
            int UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            try
            {
                if (string.IsNullOrWhiteSpace(comment))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập đánh giá > 10 kí tự. ";
                    return Json(obj);
                }

                Comment com = new Comment(UserID, koiID, DateTime.Now, null,null, comment, commentId, true);
                if (commentDao.addComment(com))
                {
                    obj.Status = 1;
                    //obj.Message = "Bạn đã đổi sang chủ " + username + " thành công vui lòng chờ xác nhận";
                    //obj.JsonObject = NewMemberID.MemberID;
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

        [System.Web.Http.HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteComment(string commentid)
        {

            Comment cmt = db.Comments.Find(int.Parse(commentid));
            cmt.Status = false;
            db.Comments.Attach(cmt);
            db.Entry(cmt).Property(x => x.Status).IsModified = true;
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



    }
}