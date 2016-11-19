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
        [HttpPost, ValidateInput(false)]
        public ActionResult AddArticle(string title, string typeid, string content, string shortdesription)
        {
            ViewBag.Message = string.Empty;
            string Imagename = String.Empty;
            var fullpath = new List<string>();
            var MaxArticleID = articleDao.GetMaxAchiID();
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
                        var filename = Path.GetFileName("Article" + typeid + MaxArticleID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        article.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Article/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Article"), filename);
                        file.SaveAs(path);
                    }
                    else if (file != null)
                    {
                        var filename = Path.GetFileName("Article" + typeid + MaxArticleID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        article.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Article/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Article"), filename);
                        file.SaveAs(path);
                    }
                }
                if (articleDao.AddArticle(article))
                {
                    ViewBag.Message = "Bạn đã thêm thành công!";
                    obj.RedirectTo = this.Url.Action("ListArticle", "Article");
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