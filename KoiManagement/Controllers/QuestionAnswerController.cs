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
            ViewBag.TypeID = new SelectList(db.Types, "TypeID", "Name");
            var type = db.Types.ToList();
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
                    obj.RedirectTo = this.Url.Action("ListQA", "QuestionAnswer");
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

        public ActionResult QuestionDetail(int? id)
        {
            //neu co id la 0 truyen vao thi ?
            if (id == 0)
            {
                return View();
            }
            var questiondetail = QADao.GetQuestionDetail((int)id);
            ViewBag.questionDetail = questiondetail;
            return View();
        }

        public ActionResult ListQA(int? id)
        {
            QuestionAnswerDAO QADao = new QuestionAnswerDAO();

            ViewBag.listIQA = QADao.GetListQuestion();
            return View();

        }
	}
}