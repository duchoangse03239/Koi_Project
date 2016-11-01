using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KoiManagement.ViewModel;
using System.IO;
using System.Linq.Expressions;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using Microsoft.Ajax.Utilities;
using PagedList;

namespace KoiManagement.Controllers
{
    public class KoiController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();
        
        /// <summary>
        /// List Koi
        /// </summary>
        List<Koi> ListKois;

        // GET: /Koi/
        public ActionResult Index(int? id)
        {
            // Mid = 2; //@@hard code filter Member id login
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            // Lấy KoiId theo người sở hưu
            var Owner = db.Owners.Where(p => p.MemberID == id);
            if (Owner.Count() > 0) Owner.ToList();
            ListKois = new List<Koi>();

            foreach (var item1 in Owner)
            {
                var kois = db.Kois.Where(p => p.KoiID == item1.KoiID).ToList();

                if (kois.Count > 0)
                {
                    ListKois.Add(kois.ElementAt(0));
                }
            }

            if (ListKois == null)
            {
                return HttpNotFound();
            }
            return View(ListKois);
        }


        // GET: /Koi/ListKoi/5
        public ActionResult ListKoi(int? id ,int? page ,string orderby, string nameKoi, string username,string variety,string sizeFrom, string sizeTo, string gender, string owner, string age)
        {
            KoiDAO kDao = new KoiDAO();
            VarietyDAO varietyDao = new VarietyDAO();
            if (id != null&& String.IsNullOrEmpty(variety))
            {
                variety = id.ToString();
            }
            if (!String.IsNullOrEmpty(variety)&&variety.Equals("0"))
            {
                variety ="";
            }

            //ViewBag.VarietyId = id;

            KoiFilterModel filter = new KoiFilterModel(orderby, nameKoi, username, variety, sizeFrom, sizeTo, gender,owner,age);
            ViewBag.Filter = filter;
            ViewBag.listVariety = varietyDao.getListMainVariety();
            var koi = db.Kois.AsQueryable();

            koi = kDao.KoiFilter(filter);

            // phân trang 6 item 1trang
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            ViewBag.Listkoi = koi.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult ListKoiFilter(string nameKoi, string sizeFrom, string sizeTo)
        {
            var listkoi = new List<Koi>();
            var Koi = db.Kois.Where(p => p.VarietyID == 4 && p.Status == true).ToList();
            foreach (var item in Koi)
            {
                if (!String.IsNullOrWhiteSpace(sizeTo) &&GetLastInfoDetail(item.KoiID).Size <= decimal.Parse(sizeTo))
                {
                    listkoi.Add(item);
                }
            }
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            obj.JsonObject = Koi;
            int pageSize = 6;
            int pageNumber = 1;
            ViewBag.Listkoi = listkoi.ToPagedList(pageNumber, pageSize);
            return Json(obj);
        }

        public InfoDetail GetLastInfoDetail(int koiId)
        {
            var infordetail = db.InfoDetails.Where(p =>p.KoiID== koiId && p.Date == db.InfoDetails.Max(j => j.Date).Value).ToList();
            var infordetail1 = db.InfoDetails.Max(j => j.Date);
            return infordetail.FirstOrDefault();
        }



        // GET: /Koi/ListKoi/5
        public ActionResult KoiUser(int id=0)
        {
            // Lấy KoiId theo người sở hưu
            if (id == 0)
            {
               return RedirectToAction("ListKoi", "Koi");
            }
            try
            {
                // id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
                KoiDAO koiDao = new KoiDAO();
                ListKois = koiDao.GetListKoiByMember(id);
                ViewBag.MemberId = id;
                if (ListKois != null)
                {
                    return View(ListKois);
                }
            }catch (Exception ){
                return RedirectToAction("SystemError", "Error");
            }
            return View();

        }
        public ActionResult TestAddd()
        {
            //@@fillter by VArity id
            //var VarrityID = 2;

            return View();
        }


        // GET: /Koi/Details/5
        public ActionResult Details(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OwnerDAO ownDao= new OwnerDAO();
            Koi koi = db.Kois.Find(id);
                // return name of owner
            ViewBag.Owner = ownDao.GetOwnerName(id);
            // Lấy giá trị deatail cuối cùng
            var KoiDeatail = db.InfoDetails.Where(p => p.KoiID == id).OrderBy(p => p.Date);
            if (KoiDeatail.Any())
            {
                KoiDeatail.First();
            }
            

            if (koi == null)
            {
                return HttpNotFound();
            }
            return View(koi);
        }

        // GET: /Koi/Create
        public ActionResult AddKoi()
        {
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName");
            var variety = db.Varieties.ToList();
            return View(variety);
        }

        // POST: /Koi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult AddKoi(string KoiName, string Size, string VarietyID, string Gender, string DoB, string Temperament, string Origin)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            // check login
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 0;
                obj.RedirectTo = Url.Action("Login", "Account");
                return Json(obj);
            }
            string Imagename = String.Empty;
            var fullpath = String.Empty;
            //validate
            try
            {
                if (!Validate.CheckLengthInput(KoiName, 0, 100))
                {
                    obj.Status = 1;
                    obj.Message = "Vui lòng nhập tên Koi!(Không quá 100 ký tự)";
                    return Json(obj);
                }
                if (!Validate.CheckSpecialCharacterInput(KoiName, @"^[a-zA-Z0-9- ]*$"))
                {
                    obj.Status = 1;
                    obj.Message = "Vui lòng không nhập ký tự đặc biệt cho tên Koi";
                    return Json(obj);
                }
                if (Decimal.Parse(Size) < 0)
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng không nhập số âm cho kích thước của Koi";
                    return Json(obj);
                }
                if (String.IsNullOrWhiteSpace(Size))
                {
                    obj.Status = 2;
                    obj.Message = "Kích thước không được để trống";
                }

                //Lấy file ảnh
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = null;
                //Trường hợp chỉ  có 1 file
                for (int i = 0; i < files.Count; i++)
                {
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  
                    file = files[i];
                }
                KoiDAO koiDao = new KoiDAO();
                decimal size;
                size = decimal.Parse(Size);
                DateTime? dateOfBirth = Validate.ConverDateTime(DoB);
                // lấy id max đặt tên file ảnh
                var MaxID = koiDao.GetMaxKoiID();
                var koi = new Koi(int.Parse(VarietyID), KoiName, dateOfBirth, Gender, Temperament, DoB, "certificate",
                    Origin, true, true);

                //set default value

                ////Save file to local
                if (file != null)
                {
                    //string path = AppDomain.CurrentDomain.BaseDirectory + "Uploads/"; 
                    Imagename = Path.GetFileName("Koi" + MaxID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                    koi.Image = Imagename;
                    fullpath = Server.MapPath("~/Content/Image/Koi/" + Imagename);
                    var path = Path.Combine(Server.MapPath("~/Content/Image/Koi"), Imagename);
                    file.SaveAs(path);

                }

                // thêm thông tin koi vào 3 bảng: koi, owner, infoDetail
                if (koiDao.AddKoi(koi, int.Parse(Session[SessionAccount.SessionUserId].ToString()), Imagename, size))
                {
                    //success
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm koi thành công";
                    obj.RedirectTo = Url.Action("KoiUser/" + Session[SessionAccount.SessionUserId], "Koi");
                    return Json(obj);
                }
                else
                {
                    //Nếu fail xoa anh da them
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                }

                ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName");
                //return View();
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

        // GET: /Koi/Edit/5
        public ActionResult EditKoi(int? id)
        {
            if (Session[SessionAccount.SessionUserId] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id == null)
            {
                return RedirectToAction("ListKoi", "Koi");
            }
            Koi koi = db.Kois.Find(id);
            if (koi == null)
            {
                return HttpNotFound();
            }
            ViewBag.VarietyID = db.Varieties;

            return View(koi);
        }



        [HttpPost]
        public JsonResult EditKoi(string KoiId,string KoiName, string Image, string VarietyID, string Gender, string DoB, string Temperament, string Origin)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            // check login
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 0;
                obj.RedirectTo = Url.Action("Login", "Account");
                return Json(obj);
            }
            try
            {
                KoiDAO koiDao = new KoiDAO();
                VarietyDAO varietyDao = new VarietyDAO();

                // Validate

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
                DateTime? dateOfBirth = Validate.ConverDateTime(DoB);
                Koi koi = new Koi(int.Parse(VarietyID),KoiName, dateOfBirth, Gender,Temperament,DoB,"",Origin,true,true);
                koi.KoiID = int.Parse(KoiId);
                koi.Image = Image;
                //Edit file to local
                if (file != null)
                {
                    if (koi.Image == null)
                    {
                        filename = Path.GetFileName("Koi" + koi.KoiID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        koi.Image = filename;
                    }
                    else
                    {
                        filename = koi.Image;
                    }
                    var fullpath = Server.MapPath("~/Content/Image/Koi/" + filename);
                    var path = Path.Combine(Server.MapPath("~/Content/Image/Koi"), filename);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    file.SaveAs(path);
                }
                //thanh cong
                if (koiDao.EditKoi(koi) == 1)
                {
                    obj.Status = 1;
                    obj.Message = "Cập nhật thông tin thành công";
                    obj.RedirectTo = Url.Action("KoiUser/" + Session[SessionAccount.SessionUserId], "Koi");
                }
            ViewBag.VarietyID = varietyDao.getListVariety();

            } catch (Exception ex){
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }


        // GET: /Koi/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Koi koi = db.Kois.Find(id);
            if (koi == null)
            {
                return HttpNotFound();
            }
            return View(koi);
        }

        // POST: /Koi/Delete/5
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string koiID)
        {
            Koi koi = db.Kois.Find(int.Parse(koiID));
            koi.Status = false;
            db.Kois.Attach(koi);
            db.Entry(koi).Property(x => x.Status).IsModified = true;
            int result= db.SaveChanges();
            //return View();
            if (result==1)
            {

               // return Json(new { result = true });
                return RedirectToAction("ListKoi/1");
            }
            else
            {
                return Json(new
                {
                    result = false
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
