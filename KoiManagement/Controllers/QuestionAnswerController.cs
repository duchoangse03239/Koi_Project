using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using PagedList;

namespace KoiManagement.Controllers
{
    public class QuestionAnswerController : Controller
    {
        MemberDAO memberDao = new MemberDAO();
        QuestionAnswerDAO QADao = new QuestionAnswerDAO();
        private KoiManagementEntities db = new KoiManagementEntities();

        /// <summary>
        /// AddArticle
        /// </summary>
        /// <returns>View</returns>
        public ActionResult CreateQuestion()
        {
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.TypeID = QADao.GetListType();
            //ViewBag.TypeID = new SelectList(db.Types, "TypeID", "Name");
            var type = QADao.GetListType();
            return View(type);
        }

        /// <summary>
        /// CreateQuestion
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="typeid">typeid</param>
        /// <param name="content">content</param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult CreateQuestion(string title, string typeid, string content)
        {
            ViewBag.Message = string.Empty;
            string Imagename = String.Empty;
            var fullpath = new List<string>();
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                RedirectToAction("Login", "Account");
                return Json(obj);
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            try
            {
                Question question = new Question();
                question.Title = title;
                question.TypeID = int.Parse(typeid);
                question.Datetime = DateTime.Now;
                question.MemberID = id;
                question.Content = content;
                question.IsClose = false;
                question.Status = true;

                if (QADao.CreateQuestion(question))
                {
                    ViewBag.Message = "Bạn đã thêm thành công!";
                    obj.RedirectTo = this.Url.Action("ListQA/"+ typeid, "QuestionAnswer");
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra xin hãy thử lại";
                    obj.RedirectTo = this.Url.Action("SystemError", "Error");
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


        /// <summary>
        /// CreateAnswer
        /// </summary>
        /// <param name="qID">qID</param>
        /// <param name="content">content</param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public JsonResult CreateAnswer(string qID, string content, string RepID)
        {
            ViewBag.Message = string.Empty;
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 2;
                obj.Message = "Xin hãy đăng nhập.";
                return Json(obj);
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            try
            {
                Answer answer = new Answer();
                answer.QuestionID = int.Parse(qID);
                answer.Datetime = DateTime.Now;
                answer.MemberID = id;
                if (!string.IsNullOrEmpty(RepID))
                {
                answer.AnswerDetail = int.Parse(RepID);
                }
                answer.Content = content;
                answer.Status = true;

                if (QADao.CreateAnswer(answer))
                {
                    obj.Status = 1;
                    ViewBag.Message = "Bạn đã thêm thành công!";
                    obj.RedirectTo = this.Url.Action("ListQA", "QuestionAnswer");
                }
                else
                {
                    obj.Status = 0;
                    ViewBag.Message = "Có lỗi xảy ra xin hãy thử lại";
                    obj.RedirectTo = this.Url.Action("SystemError", "Error");
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


        [HttpPost]
        public JsonResult RateQ(int QId, string RateNum)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 2;
                obj.Message = "Xin hãy đăng nhập.";
                return Json(obj);
            }
            try
            {
                // lấy id người đang đăng nhập
                int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
                if (!QADao.CheckRateQ(id, QId))
                {
                    obj.Status = 2;
                    obj.Message = "Bạn đã đánh giá.";
                    return Json(obj);
                }
                Rate rate = new Rate();
                rate.QuestionID = QId;
                rate.MemberID = id;
                rate.RateSocre = int.Parse(RateNum);

                db.Rates.Add(rate);
                db.SaveChanges();

                obj.Status = 1;
                obj.Message = "Đánh giá thành công.";
                return Json(obj);
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                return Json(obj);
            }
        }

        [HttpPost]
        public JsonResult RateA(int AId, string RateNum)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 2;
                obj.Message = "Xin hãy đăng nhập.";
                return Json(obj);
            }
            try
            {


                // lấy id người đang đăng nhập
                int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
                if (!QADao.CheckRateA(id, AId))
                {
                    obj.Status = 2;
                    obj.Message = "Bạn đã đánh giá.";
                    return Json(obj);
                }

                Rate rate = new Rate();
                rate.AnswerID = AId;
                rate.MemberID = id;
                rate.RateSocre = int.Parse(RateNum);

                db.Rates.Add(rate);
                db.SaveChanges();

                obj.Status = 1;
                obj.Message = "Đánh giá thành công.";
                return Json(obj);
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                return Json(obj);
            }
        }

        public ActionResult QuestionDetail(int? id)
        {
            //neu co id la 0 truyen vao thi ?
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            var questiondetail = QADao.GetQuestionDetail((int)id);
            if (questiondetail==null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ViewBag.answer = QADao.GetListAnswerbyId(id.Value);
            ViewBag.ListAnswerDetail = QADao.GetListAnswerDetail(id.Value);

            ViewBag.questionDetail = questiondetail;
            ViewBag.questionId = id;
            return View();
        }

        public ActionResult ListQA(int? id, int? page)
        {
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            var type = db.Types.Find(id.Value);
            if (type == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            var ListQA = QADao.GetListQuestion(id.Value);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListQA = ListQA.ToPagedList(pageNumber, pageSize);
            return View();
        }

        public ActionResult ListType(int? page)
        {
            var ListType = QADao.GetListType();
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            ViewBag.ListType = ListType.ToPagedList(pageNumber, pageSize);
            return View();
        }
    }
}