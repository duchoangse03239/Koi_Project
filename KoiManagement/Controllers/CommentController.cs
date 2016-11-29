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
        CommentDAO commentDao= new CommentDAO();
        // GET: Comment
        public ActionResult Index()
        {
            return View();
        }
        [System.Web.Mvc.HttpPost]
        public JsonResult AddCommentKoi([FromBody] string Comment, int koiID ,decimal RateNum)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            //int UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            int UserID = 2;
            try
            {
                if (string.IsNullOrWhiteSpace(Comment))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập đánh giá > 10 kí tự. ";
                    return Json(obj);
                }

                Comment com = new Comment(UserID, koiID, DateTime.Now,null, RateNum, Comment,null,true);
                Comment comtsest = commentDao.GetCommentbyId(5);
               // if (commentDao.addComment(com))
                {
                    obj.Status = 1;
                    obj.JsonObject = comtsest;
                    //obj.Message = "Bạn đã đổi sang chủ " + username + " thành công vui lòng chờ xác nhận";
                    //obj.JsonObject = NewMemberID.MemberID;
                    // ownerDao.ChangeOwner(username, int.Parse(koiId))
                    ViewBag.NewComment = comtsest;
                    return Json(com);
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
            //int UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            int UserID = 2;
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
    }
}