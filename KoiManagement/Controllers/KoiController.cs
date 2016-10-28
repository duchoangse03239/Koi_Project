﻿using System;
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
using Model.DAO;
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
        public ActionResult ListKoi(int? id ,int? page ,string orderby, string nameKoi,string variety,string sizeFrom, string sizeTo, string gender, string owner)
        {
            KoiDAO kDao = new KoiDAO();
            if (id != null&& String.IsNullOrEmpty(variety))
            {
                variety = id.ToString();
            }
            if (!String.IsNullOrEmpty(variety)&&variety.Equals("0"))
            {
                variety ="";
            }

            //ViewBag.VarietyId = id;

            KoiFilterModel filter = new KoiFilterModel(orderby, nameKoi, variety, sizeFrom, sizeTo, gender,owner);
            ViewBag.Filter = filter;
            var listkoi = new List<Koi>();

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
        public JsonResult AddKoi( string KoiName, string Size, string VarietyID, string Gender, string DoB, string Temperament, string Origin)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            if (Session[SessionAccount.SessionUserId] == null)
            {
               // return RedirectToAction("Login", "Account");
            }
            //validate
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
                decimal size ;
                size = decimal.Parse(Size);
                DateTime? dateOfBirth = Validate.ConverDateTime(DoB);
                // lấy id max đặt tên file ảnh
                var MaxID = koiDao.GetMaxKoiID();
                var koi = new Koi(int.Parse(VarietyID), KoiName, dateOfBirth, Gender, Temperament, DoB, "certificate", Origin, true, true);
                
                //set default value
                string Imagename=String.Empty;
                var fullpath = String.Empty;
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
                if (koiDao.AddKoi(koi, int.Parse(Session[SessionAccount.SessionUserId].ToString()), Imagename ,size ))
                {
                    //success
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm koi thành công";
                    obj.RedirectTo = Url.Action("KoiUser", "Koi");
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
            return Json(obj);
        }

        // GET: /Koi/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.VarietyID = db.Varieties;

            return View(koi);
        }

        // POST: /Koi/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit(HttpPostedFileBase file, [Bind(Include = "KoiId,VarietyID,Image,KoiName,DoB,Gender,Temperament,Origin")] Koi koi)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string filename;

        //        //Edit file to local
        //        if (file != null)
        //        {
        //            if (koi.Image == null)
        //            {
        //                filename = Path.GetFileName("Koi" + koi.KoiID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
        //                koi.Image = filename;
        //            }
        //            else
        //            {
        //                filename = koi.Image;
        //            }
        //            var fullpath = Server.MapPath("~/Content/Image/Koi/" + filename);
        //            var path = Path.Combine(Server.MapPath("~/Content/Image/Koi"), filename);
        //            if(System.IO.File.Exists(fullpath)){
        //               System.IO.File.Delete(fullpath);
        //            }
        //            file.SaveAs(path);
        //        }

        //        db.Kois.Attach(koi);
        //        var entry = db.Entry(koi);
        //        entry.State = EntityState.Modified;
        //        // Set column not change
        //        entry.Property(e => e.Status).IsModified = false;
        //        entry.Property(e => e.Privacy).IsModified = false;
        //        db.SaveChanges();
        //        return RedirectToAction("ListKoi/" + koi.VarietyID);
        //    }
        //    ViewBag.VarietyID = db.Varieties;
        //    return View(koi);
        //}

        [HttpPost]
        public ActionResult Edit(HttpPostedFileBase file, [Bind(Include = "KoiId,VarietyID,Image,KoiName,DoB,Gender,Temperament,Origin")] Koi koi)
        {
            if (ModelState.IsValid)
            {
                string filename;

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

                db.Kois.Attach(koi);
                var entry = db.Entry(koi);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.Status).IsModified = false;
                entry.Property(e => e.Privacy).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("ListKoi/" + koi.VarietyID);
            }
            ViewBag.VarietyID = db.Varieties;
            return View(koi);
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
