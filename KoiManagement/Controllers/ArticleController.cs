using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace KoiManagement.Controllers
{
    public class ArticleController : Controller
    {
        MemberDAO memberDao = new MemberDAO();
        ArticleDAO articleDao = new ArticleDAO();
        private KoiManagementEntities db = new KoiManagementEntities();

        //
        // GET: /Account/
        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login
        public ActionResult AddArticle()
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

        // POST: /Account/Login
        [HttpPost]
        public ActionResult AddArticle(string title, string typeid, string date, string content)
        {
            ViewBag.Message = string.Empty;
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            try
            {
                Article article = new Article();
                article.Title = title;
                article.TypeID = int.Parse(typeid);
                article.Date = DateTime.Now;
                article.MemberID = id;
                article.Content = content;
                if (articleDao.AddArticle(article))
                {
                    ViewBag.Message = "Bạn đã thêm thành công, vui long chờ người quản trị xác nhận thông tin";
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra xin hãy thử lại";
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


        public ActionResult ListArticle(int id, int? page)
        {
            var ListArticle = articleDao.GetListArticle(id);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListArticle = ListArticle.ToPagedList(pageNumber, pageSize);
            return View();
        }

    }
}