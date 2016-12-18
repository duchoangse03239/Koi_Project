using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using PagedList;

namespace KoiManagement.Controllers
{
    public class AdminController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();
        AdminDAO adminDao = new AdminDAO();
        MemberDAO memberDao = new MemberDAO();
        KoiDAO koiDao = new KoiDAO();
        KoiFarmDAO koiFarmDao = new KoiFarmDAO();
        ArticleDAO articleDao = new ArticleDAO();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Home()
        {
            ViewBag.home = db.Articles.Where(p => p.Type.Status == 0).FirstOrDefault();
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditArticle( string content, string articleid)
        {
            ViewBag.Message = string.Empty;
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            //if (Session[SessionAccount.SessionUserId] == null)
            //{
            //    RedirectToAction("Login", "Account");
            //    return Json(obj);
            //}
            // lấy id người đang đăng nhập
            //int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            ////Viewbag cho patialView _Manager
            //ViewBag.Member = memberDao.GetMemberbyID(id);
            try
            {
                Article article = new Article();

  

                Article mem = db.Articles.Find(int.Parse(articleid));
                mem.Content = content;
                db.Articles.Attach(mem);
                db.Entry(mem).Property(x => x.Content).IsModified = true;
                int result = db.SaveChanges();


                if (result > 0)
                {
                    ViewBag.Message = "Bạn đã sửa thành công!";
                    obj.RedirectTo = this.Url.Action("Home", "Admin");
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


        public ActionResult ListMember(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ////kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.UserName = String.IsNullOrEmpty(sortOrder) ? "username_desc" : "";
            ViewBag.Name = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.Phone = sortOrder == "Phone" ? "Phone_desc" : "Phone";
            ViewBag.Email = sortOrder == "Email" ? "Email_desc" : "Phone";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";
            ViewBag.Role = sortOrder == "Role" ? "AllRole" : "Role";

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            IQueryable<Member> Member = db.Members.AsQueryable();
           // Member = Member.OrderBy(p => p.Name);
            //search
            Member = adminDao.getlistMember(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListMember = Member.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }

        [HttpPost]
        public ActionResult DeActiveMember(string MemberID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Member mem = db.Members.Find(int.Parse(MemberID));
            mem.Status = false;
            db.Members.Attach(mem);
            db.Entry(mem).Property(x => x.Status).IsModified = true;
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
        public ActionResult ActiveMember(string MemberID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Member mem = db.Members.Find(int.Parse(MemberID));
            mem.Status = true;
            db.Members.Attach(mem);
            db.Entry(mem).Property(x => x.Status).IsModified = true;
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
        public ActionResult MakeAdmin(string MemberID)
        {
            Member mem = db.Members.Find(int.Parse(MemberID));
            mem.Role = 1;
            db.Members.Attach(mem);
            db.Entry(mem).Property(x => x.Role).IsModified = true;
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
        public ActionResult RemoveAdmin(string MemberID)
        {
            Member mem = db.Members.Find(int.Parse(MemberID));
            mem.Role = null;
            db.Members.Attach(mem);
            db.Entry(mem).Property(x => x.Role).IsModified = true;
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
        public ActionResult ListKoi(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.KoiId = String.IsNullOrEmpty(sortOrder) ? "koiID_desc" : "";
            ViewBag.Variety = sortOrder == "Variety" ? "Variety_desc" : "Variety";
            ViewBag.KoiName = sortOrder == "KoiName" ? "KoiName_desc" : "KoiName";
            ViewBag.Dob = sortOrder == "Dob" ? "Dob_desc" : "Dob";
            ViewBag.Owner = sortOrder == "Owner" ? "Owner_desc" : "Owner";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;

            IQueryable<Koi> Koi = db.Kois.AsQueryable();
            // Member = Member.OrderBy(p => p.Name);
            //search
            Koi = adminDao.getlistKoi(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListKoi = Koi.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }
        public ActionResult ListKoiFarm(string sortOrder, string currentFilter, string searchString, int? page)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.FarmName = String.IsNullOrEmpty(sortOrder) ? "FarmName_desc" : "";
            ViewBag.Name = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.Phone = sortOrder == "Phone" ? "Phone_desc" : "Phone";
            ViewBag.Address = sortOrder == "Address" ? "Address_desc" : "Address";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";
            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;

            IQueryable<KoiFarm> KoiFarm = db.KoiFarms.AsQueryable();
            // KoiFarm = KoiFarm.OrderBy(p => p.Name);
            //search
            KoiFarm = adminDao.getlistKoiFarm(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListKoiFarm = KoiFarm.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }


        [HttpPost]
        public ActionResult ActiveKoi(string KoiID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Koi koi = db.Kois.Find(int.Parse(KoiID));
            koi.Status = 1;
            db.Kois.Attach(koi);
            db.Entry(koi).Property(x => x.Status).IsModified = true;
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
        public ActionResult DeActiveKoi(string KoiID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Koi koi = db.Kois.Find(int.Parse(KoiID));
            koi.Status = -1;
            db.Kois.Attach(koi);
            db.Entry(koi).Property(x => x.Status).IsModified = true;
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
        public ActionResult DeActiveKoiFarm(string koifarmId)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            KoiFarm mem = db.KoiFarms.Find(int.Parse(koifarmId));
            mem.Status = -1;
            db.KoiFarms.Attach(mem);
            db.Entry(mem).Property(x => x.Status).IsModified = true;
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
        public ActionResult ActiveKoiFarm(string koifarmId)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            KoiFarm mem = db.KoiFarms.Find(int.Parse(koifarmId));
            mem.Status = 1;
            db.KoiFarms.Attach(mem);
            db.Entry(mem).Property(x => x.Status).IsModified = true;
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
        public ActionResult DeActiveReport(string ReportID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Report re = db.Reports.Find(int.Parse(ReportID));
            re.Status = false;
            db.Reports.Attach(re);
            db.Entry(re).Property(x => x.Status).IsModified = true;
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
        public ActionResult DeActiveReportMember(string MemberID)
        {
            int id = int.Parse(MemberID);
            var listReport = db.Reports.Where(p => p.ObjectType == "Member" && p.ObjectId == id && p.Status);
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var report in listReport)
                    {
                    Report re = report;
                    re.Status = false;
                    db.Reports.Attach(re);
                    db.Entry(re).Property(x => x.Status).IsModified = true;
                    db.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                }
            }
            DeActiveMember(MemberID);
            return Json(new { result = true });
        }
        [HttpPost]
        public ActionResult DeActiveReportKoi(string KoiID)
        {
            int id = int.Parse(KoiID);
            var listReport = db.Reports.Where(p => p.ObjectType == "Koi" && p.ObjectId == id && p.Status);
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var report in listReport)
                    {
                        Report re = report;
                        re.Status = false;
                        db.Reports.Attach(re);
                        db.Entry(re).Property(x => x.Status).IsModified = true;
                        db.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                }
            }
            DeActiveKoi(KoiID);
            return Json(new { result = true });
        }
        [HttpPost]
        public ActionResult DeActiveReportKoiFarm(string KoifarmID)
        {
            int id = int.Parse(KoifarmID);
            var listReport = db.Reports.Where(p => p.ObjectType == "KoiFarm" && p.ObjectId == id && p.Status);
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var report in listReport)
                    {
                        Report re = report;
                        re.Status = false;
                        db.Reports.Attach(re);
                        db.Entry(re).Property(x => x.Status).IsModified = true;
                        db.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                }
            }
            DeActiveKoiFarm(KoifarmID);
            return Json(new { result = true });
        }

        [HttpPost]
        public ActionResult DeActiveReportQuestion(string QuestionID)
        {
            int id = int.Parse(QuestionID);
            var listReport = db.Reports.Where(p => p.ObjectType == "Question" && p.ObjectId == id && p.Status);
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var report in listReport)
                    {
                        Report re = report;
                        re.Status = false;
                        db.Reports.Attach(re);
                        db.Entry(re).Property(x => x.Status).IsModified = true;
                        db.SaveChanges();
                    }
                    Question mem = db.Questions.Find(int.Parse(QuestionID));
                    mem.Status = false;
                    db.Questions.Attach(mem);
                    db.Entry(mem).Property(x => x.Status).IsModified = true;
                    int result = db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                }
            }
            return Json(new { result = true });
        }

        [HttpPost]
        public ActionResult DeActiveReportAnswer(string AnswerID)
        {
            int id = int.Parse(AnswerID);
            var listReport = db.Reports.Where(p => p.ObjectType == "Answer" && p.ObjectId == id && p.Status);
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (var report in listReport)
                    {
                        Report re = report;
                        re.Status = false;
                        db.Reports.Attach(re);
                        db.Entry(re).Property(x => x.Status).IsModified = true;
                        db.SaveChanges();
                    }
                    Answer mem = db.Answers.Find(int.Parse(AnswerID));
                    mem.Status = false;
                    db.Answers.Attach(mem);
                    db.Entry(mem).Property(x => x.Status).IsModified = true;
                    int result = db.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                }
            }
            return Json(new { result = true });
        }


        public ActionResult MemberReport(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ////kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.Date = String.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";
            ViewBag.UserName = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.UserName1 = sortOrder == "UserName1" ? "UserName1_desc" : "UserName1";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            IQueryable<Report> Member = db.Reports.AsQueryable();
            // Member = Member.OrderBy(p => p.Name);
            //search
            Member = adminDao.MemberReport(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListMember = Member.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }

        public ActionResult KoiReport(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ////kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.Date = String.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";
            ViewBag.UserName = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.KoiID = sortOrder == "KoiID" ? "KoiID_desc" : "KoiID";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            IQueryable<Report> Koi = db.Reports.AsQueryable();
            // Member = Member.OrderBy(p => p.Name);
            //search
            Koi = adminDao.KoiReport(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListMember = Koi.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }
        public ActionResult KoiFarmReport(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ////kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.Date = String.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";
            ViewBag.UserName = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.KoiFarmID = sortOrder == "KoiFarmID" ? "KoiFarmID_desc" : "KoiFarmID";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            IQueryable<Report> Koi = db.Reports.AsQueryable();
            // Member = Member.OrderBy(p => p.Name);
            //search
            Koi = adminDao.KoiFarmReport(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListMember = Koi.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }


        public ActionResult QuestionReport(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ////kiem tra phan quyen
            //if (Session[SessionAccount.SessionUserId] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            //var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            //if (mem.Role != 1)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.Date = String.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";
            ViewBag.UserName = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.QuestionID = sortOrder == "QuestionID" ? "QuestionID_desc" : "QuestionID";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            IQueryable<Report> QA = db.Reports.AsQueryable();
            // Member = Member.OrderBy(p => p.Name);
            //search
            QA = adminDao.QuesTionReport(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListMember = QA.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }

        public ActionResult AnswerReport(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ////kiem tra phan quyen
            //if (Session[SessionAccount.SessionUserId] == null)
            //{
            //    return RedirectToAction("Login", "Account");
            //}
            //var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            //if (mem.Role != 1)
            //{
            //    return RedirectToAction("Index", "Home");
            //}
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.Date = String.IsNullOrEmpty(sortOrder) ? "Date_desc" : "";
            ViewBag.UserName = sortOrder == "UserName" ? "UserName_desc" : "UserName";
            ViewBag.AnswerID = sortOrder == "AnswerID" ? "AnswerID_desc" : "AnswerID";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";

            ViewBag.CurrentFilter = searchString;
            ViewBag.CurrentSort = sortOrder;
            IQueryable<Report> QA = db.Reports.AsQueryable();
            // Member = Member.OrderBy(p => p.Name);
            //search
            QA = adminDao.AnswerReport(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListMember = QA.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }

        public ActionResult ListArticle(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ////kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.KoiId = String.IsNullOrEmpty(sortOrder) ? "koiID_desc" : "";
            ViewBag.Variety = sortOrder == "Variety" ? "Variety_desc" : "Variety";
            ViewBag.KoiName = sortOrder == "KoiName" ? "KoiName_desc" : "KoiName";
            ViewBag.Dob = sortOrder == "Dob" ? "Dob_desc" : "Dob";
            ViewBag.Owner = sortOrder == "Owner" ? "Owner_desc" : "Owner";
            ViewBag.Status = sortOrder == "Status" ? "AllStatus" : "Status";

            ViewBag.CurrentSort = sortOrder;
            ViewBag.CurrentFilter = searchString;

            IQueryable<Article> Koi = db.Articles.AsQueryable();
            // Member = Member.OrderBy(p => p.Name);
            //search
            Koi = adminDao.getlistArticle(searchString, sortOrder);
            int pageSize = 7;
            int pageNumber = (page ?? 1);
            ViewBag.ListArticle = Koi.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }
        [HttpPost]
        public ActionResult DeActiveArticle(string ArticleID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Article mem = db.Articles.Find(int.Parse(ArticleID));
            mem.Status = false;
            db.Articles.Attach(mem);
            db.Entry(mem).Property(x => x.Status).IsModified = true;
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
        public ActionResult ActiveArticle(string ArticleID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Article mem = db.Articles.Find(int.Parse(ArticleID));
            mem.Status = true;
            db.Articles.Attach(mem);
            db.Entry(mem).Property(x => x.Status).IsModified = true;
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
        public ActionResult ActiveQuestion(string QuestionID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Question mem = db.Questions.Find(int.Parse(QuestionID));
            mem.Status = true;
            db.Questions.Attach(mem);
            db.Entry(mem).Property(x => x.Status).IsModified = true;
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
        public ActionResult ActiveAnswer(string AnswerID)
        {
            //kiem tra phan quyen
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var mem1 = memberDao.GetMemberbyID(int.Parse(Session[SessionAccount.SessionUserId].ToString()));
            if (mem1.Role != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            Answer mem = db.Answers.Find(int.Parse(AnswerID));
            mem.Status = true;
            db.Answers.Attach(mem);
            db.Entry(mem).Property(x => x.Status).IsModified = true;
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