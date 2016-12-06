using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.IO;

namespace KoiManagement.Controllers
{
    public class ArticleController : Controller
    {
        MemberDAO memberDao = new MemberDAO();
        ArticleDAO articleDao = new ArticleDAO();
        private KoiManagementEntities db = new KoiManagementEntities();

        /// <summary>
        /// ActionResult
        /// </summary>
        /// <returns>View</returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// AddArticle
        /// </summary>
        /// <returns>View</returns>
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

        /// <summary>
        /// AddArticle
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="typeid">typeid</param>
        /// <param name="content">content</param>
        /// <param name="shortdesription">shortdesription</param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult AddArticle(string title, string typeid, string content, string shortdesription)
        {
            ViewBag.Message = string.Empty;
            var fullpath = new List<string>();
            var MaxArticleID = articleDao.GetMaxAchiId();
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
                Article article = new Article();
                article.Title = title;
                article.TypeID = int.Parse(typeid);
                article.Date = DateTime.Now;
                article.MemberID = id;
                article.ShortDes = shortdesription;
                article.Content = content;
                article.Status = true;
                //Lấy file ảnh
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = null;
                //Trường hợp chỉ  có 1 file

                for (int i = 0; i < files.Count; i++)
                {
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  
                    file = files[i];
                    //Save file to local
                    if (file != null && i == 0)
                    {
                        var filename = Path.GetFileName("Article" + MaxArticleID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        article.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Article/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Article"), filename);
                        file.SaveAs(path);
                    }
                    else if (file != null)
                    {
                        var filename = Path.GetFileName("Article" + MaxArticleID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        article.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Article/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Article"), filename);
                        file.SaveAs(path);
                    }
                }
                if (articleDao.AddArticle(article))
                {
                    ViewBag.Message = "Bạn đã thêm thành công!";
                    obj.RedirectTo = this.Url.Action("ListArticle", "Admin");
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

        public ActionResult EditArticle(int id)
        {
            //if (Session[SessionAccount.SessionUserId] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            // lấy id người đang đăng nhập
           // int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
           // ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.TypeID = new SelectList(db.Types, "TypeID", "Name");
            var articledetail = articleDao.GetArticleDetail((int)id);
            ViewBag.Article = articledetail;
            var type = db.Types.ToList();
            return View(type);
        }

        /// <summary>
        /// EditArticle
        /// </summary>
        /// <param name="title">title</param>
        /// <param name="typeid">typeid</param>
        /// <param name="content">content</param>
        /// <param name="shortdesription">shortdesription</param>
        /// <returns></returns>
        [HttpPost, ValidateInput(false)]
        public ActionResult EditArticle(string title, string typeid, string content, string shortdesription, string articleid, string image)
        {
            ViewBag.Message = string.Empty;
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
                Article article = new Article();
                article.Title = title;
                article.TypeID = int.Parse(typeid);
                article.Date = DateTime.Now;
                article.MemberID = id;
                article.ShortDes = shortdesription;
                article.Content = content;
                article.Status = true;
                article.ArticleID = int.Parse(articleid);
                article.Image = image;

                string filename;
                //Lấy file ảnh
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = null;
                //Trường hợp chỉ  có 1 file
                for (int i = 0; i < files.Count; i++)
                {
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  
                    file = files[i];
                }

                //Edit file to local
                if (file != null)
                {
                    if (string.IsNullOrWhiteSpace(article.Image))
                    {
                        filename = Path.GetFileName("Article" + articleid + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        article.Image = filename;
                    }
                    else
                    {
                        filename = article.Image;
                    }
                    var fullpath = Server.MapPath("~/Content/Image/Article/" + filename);
                    var path = Path.Combine(Server.MapPath("~/Content/Image/Article"), filename);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    file.SaveAs(path);
                }

                if (articleDao.EditArticle(article) > 0)
                {
                    ViewBag.Message = "Bạn đã sửa thành công!";
                    obj.RedirectTo = this.Url.Action("ListArticle", "Admin");
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

        public ActionResult ArticleDetail(int? id)
        {
            //neu co id la 0 truyen vao thi ?
            if (id == 0)
            {
                return View();
            }
            var articledetail = articleDao.GetArticleDetail((int)id);
            ViewBag.articleDetail = articledetail;
            return View();
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