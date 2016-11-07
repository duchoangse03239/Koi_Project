using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using PagedList;

namespace KoiManagement.Controllers
{
    public class KoiFarmController : Controller
    {
        KoiManagementEntities db = new KoiManagementEntities();
        KoiDAO koiDao = new KoiDAO();
        MemberDAO memberDao = new MemberDAO();
        KoiFarmDAO koiFarmDao = new KoiFarmDAO();

        // GET: KoiFarm
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult KoiFarmDetail(int? id)
        {
            if (id == 0)
            {
                return View();
            }
            var listkoi = koiFarmDao.GetListKoiByKoiFarmId((int) id);
            var koifarm = koiFarmDao.GetKoiFarmDetail((int) id);
            ViewBag.koiFarm = koifarm;
            ViewBag.listKoi = listkoi;
            return View();
        }

        public ActionResult CreateKoiFarm()
        {
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CreateKoiFarm(string farmName, string address, string description)
        {
            ViewBag.Message = string.Empty;
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            if (string.IsNullOrWhiteSpace(farmName) || string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(description) || farmName.Length > 100)
            {
                return View();

            }
            try
            {
                KoiFarm koiFarm = new KoiFarm();
                koiFarm.FarmName = farmName;
                koiFarm.Address = address;
                koiFarm.Description = description;
                koiFarm.MemberID = id;
                koiFarm.Status = true;
                if (koiFarmDao.AddKoiFarm(koiFarm))
                {
                    ViewBag.Message = "Bạn đã thêm thành công, vui long chờ người quản trị xác nhận thông tin";
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra xin hãy thử lại";
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " +
                                       validationError.ErrorMessage);
                    }
                }
            }

            return View();
        }

        public ActionResult ListKoiFarm(int? page)
        {
            var ListKoiFarm = koiFarmDao.GetListKoiFarm();
            // ViewBag.ListKoiFarm = ListKoiFarm;
            Dictionary<int, string> dict = new Dictionary<int, string>();
            List<string> listVariety = new List<string>();
            string t = "";
            foreach (var koifarm in ListKoiFarm)
            {
                t = "";
                dict.Clear();
                var db1 =
                    db.Kois.Where(p => p.Owners.Where(o => o.Status).FirstOrDefault().KoiFarmID == koifarm.KoifarmID);
                foreach (var item in db1)
                {
                    if (!dict.ContainsKey(item.Variety.VarietyID) && !item.Variety.VarietyName.Equals("Chưa rõ"))
                        dict.Add(item.Variety.VarietyID, item.Variety.VarietyName);
                }
                foreach (var v in dict)
                {
                    t += v.Value + ", ";
                }
                t = CommonFunction.Trim2LastCharacter(t);
                listVariety.Add(t);
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.listVariety = listVariety;
            ViewBag.ListKoiFarm = ListKoiFarm.ToList().ToPagedList(pageNumber, pageSize);

            return View();
        }

        public ActionResult EditKoiFarm(int? id)
        {
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            //check tồn tại id
            if (id == null)
            {
                return RedirectToAction("ListKoiFarm", "KoiFarm");
            }

            // lấy id người đang đăng nhập
            int Mid = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(Mid);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(Mid);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(Mid);

            ViewBag.Koifarm = koiFarmDao.GetKoiFarmDetail((int) id);
            return View();
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult EditKoiFarm(string farmName, string address, string description, string KoifarmID)
        {
            ViewBag.Message = string.Empty;
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            if (string.IsNullOrWhiteSpace(farmName) || string.IsNullOrWhiteSpace(address) ||
                string.IsNullOrWhiteSpace(description) || farmName.Length > 100)
            {
                return View();

            }
            try
            {
                KoiFarm koiFarm = new KoiFarm();
                koiFarm.FarmName = farmName;
                koiFarm.Address = address;
                koiFarm.Description = description;
                koiFarm.Status = true;
                koiFarm.KoifarmID = int.Parse(KoifarmID);
                if (koiFarmDao.EditKoiFarm(koiFarm) > 0)
                {
                    ViewBag.Message = "Bạn đã sửa thông tin thành công, vui long chờ người quản trị xác nhận thông tin";
                    ViewBag.Koifarm = koiFarmDao.GetKoiFarmDetail(int.Parse(KoifarmID));
                }
                else
                {
                    ViewBag.Koifarm = koiFarmDao.GetKoiFarmDetail(int.Parse(KoifarmID));
                    ViewBag.Message = "Có lỗi xảy ra xin hãy thử lại";
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " +
                                       validationError.ErrorMessage);
                    }
                }
            }

            return View();
        }

        public ActionResult KoifarmOwner()
        {
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            // lấy id người đang đăng nhập
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            var koifarm = koiFarmDao.GetListKoiFarmByMemberId(id);
            //Viewbag cho patialView _Manager
            ViewBag.Member = memberDao.GetMemberbyID(id);
            ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
            ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
            return View(koifarm);
        }

        public ActionResult AddKoiToKoiFarm(int? id)
        {
            //kiểm tra đăng nhập
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            var ad= db.Kois.Where(cu => cu.Owners.Any(c => c.MemberID == id));
            return View(ad);
        }
    }
}