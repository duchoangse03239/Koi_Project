using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;

namespace KoiManagement.Controllers
{
    public class InfoDetailController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();

        // GET: InfoDetail
        public ActionResult Index()
        {
            var infoDetails = db.InfoDetails.Include(i => i.Koi);
            return View(infoDetails.ToList());
        }

        // GET: InfoDetail/Details/5
        public ActionResult KoiInfoDetail(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var infoDetail = db.InfoDetails.Where(p => p.KoiID == id).OrderByDescending(p => p.Date);
            if (infoDetail == null)
            {
                return HttpNotFound();
            }
            return View(infoDetail.ToList());
        }

        // GET: InfoDetail/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InfoDetail infoDetail = db.InfoDetails.Find(id);
            if (infoDetail == null)
            {
                return HttpNotFound();
            }
            return View(infoDetail);
        }

        // GET: InfoDetail/add new koi
        public ActionResult AddDetail(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ViewBag.KoiID = id;
            return View();
        }

        // POST: InfoDetail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult AddDetail(HttpPostedFileBase file, [Bind(Include = "KoiId,Weight,Size,HeathyStatus,Description,Image")] InfoDetail infoDetail)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // lấy id max đặt tên file ảnh
        //        var MaxID = db.InfoDetails.Max(g => g.DetailID) + 1;

        //        //set default value
        //        infoDetail.Date = DateTime.Now;

        //        //Save file to local
        //        if (file != null)
        //        {
        //            //String id = db.Kois.SqlQuery(@"SELECT IDENT_CURRENT ('Koi') AS AnimalID").First().ToString();
        //            var filename = Path.GetFileName("Detail" + MaxID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
        //            infoDetail.Image = filename;
        //            var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
        //            file.SaveAs(path);
        //        }


        //        db.InfoDetails.Add(infoDetail);
        //        db.SaveChanges();
        //        return RedirectToAction("KoiInfoDetail/"+infoDetail.KoiID);
        //    }
            
        //    ViewBag.KoiID = new SelectList(db.Kois, "KoiID", "KoiName", infoDetail.KoiID);
        //    return View(infoDetail);
        //}


        [HttpPost]
        public JsonResult AddDetail(string KoiID, string Weight, string Size, string HeathyStatus, string Description)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            var fullpath = new List<string>();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 0;
                obj.RedirectTo = Url.Action("Login", "Account");
                return Json(obj);
            }
            try
            {
                InfoDetailDAO infoDetailDao = new InfoDetailDAO();
                MediaDAO mediaDao = new MediaDAO();
                var infoDetail = new InfoDetail();
                infoDetail.Weight = decimal.Parse(Weight);
                infoDetail.Size = decimal.Parse(Size);
                infoDetail.HeathyStatus = HeathyStatus;
                infoDetail.Description = Description;
                infoDetail.KoiID = int.Parse(KoiID);
                infoDetail.Status = true;
                // lấy id max đặt tên file ảnh
                var MaxID = db.InfoDetails.Max(g => g.DetailID) + 1;

                //Lấy file ảnh
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = null;
                //Trường hợp chỉ  có 1 file
                for (int i = 0; i < files.Count; i++)
                {
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  
                    file = files[i];
                    //Save file to local
                    if (file != null && i==0)
                    {
                        var filename = Path.GetFileName("Detail" + MaxID + i + file.FileName.Substring(file.FileName.LastIndexOf('.')) );
                        infoDetail.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
                        file.SaveAs(path);
                    }
                    else if (file != null )
                    {
                        var filename = Path.GetFileName("Detail" + MaxID + i + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        infoDetail.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
                        file.SaveAs(path);
                        Medium a =  new Medium();
                        a.ModelTypeID = "InfoDetail";
                        a.LinkImage = filename;
                        a.ModelId = infoDetail.DetailID;
                        mediaDao.addMedia(a);
                    }
                }
                //set default value
                infoDetail.Date = DateTime.Now;

                // return RedirectToAction("KoiInfoDetail/"+infoDetail.KoiID);

                ViewBag.KoiID = new SelectList(db.Kois, "KoiID", "KoiName", infoDetail.KoiID);
                 if (infoDetailDao.AddInfoDetail(infoDetail))
                {
                    //success
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm";
                    obj.RedirectTo = Url.Action("KoiInfoDetail/" + KoiID, "InfoDetail");
                    return Json(obj);
                }
                else
                {
                    //Nếu fail xoa anh da them
                    foreach (var item in fullpath)
                    {
                        if (System.IO.File.Exists(item))
                        {
                            System.IO.File.Delete(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                //Nếu fail xoa anh da them
                foreach (var item in fullpath)
                {
                    if (System.IO.File.Exists(item))
                    {
                        System.IO.File.Delete(item);
                    }
                }
                return Json(obj);
            }
            return Json(obj);
        }

        // GET: InfoDetail/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InfoDetail infoDetail = db.InfoDetails.Find(id);
            if (infoDetail == null || infoDetail.DetailID ==null)
            {
                return HttpNotFound();
            }
            return View(infoDetail);
        }

        // POST: InfoDetail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HttpPostedFileBase file, [Bind(Include = "DetailID,KoiID,Weight,Image,Size,HeavyStatus,Description")] InfoDetail infoDetail)
        {
            if (ModelState.IsValid)
            {
                string filename;

                //Edit file to local
                if (file != null)
                {
                    if (infoDetail.Image == null)
                    {
                        filename = Path.GetFileName("Koi" + infoDetail.KoiID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        infoDetail.Image = filename; 
                    }
                    else
                    {
                        filename = infoDetail.Image;
                    }
                    var fullpath = Server.MapPath("~/Content/Image/Detail/" + filename);
                    var path = Path.Combine(Server.MapPath("~/Content/Image/Detail/"), filename);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    file.SaveAs(path);
                }
                infoDetail.Date = DateTime.Now; 
                db.InfoDetails.Attach(infoDetail);
                var entry = db.Entry(infoDetail);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.KoiID).IsModified = false;
                db.SaveChanges();
                return RedirectToAction("KoiInfoDetail/" + infoDetail.KoiID);
            }
            ViewBag.KoiID = new SelectList(db.Kois, "KoiID", "KoiName", infoDetail.KoiID);
            return View(infoDetail);
        }

        // GET: InfoDetail/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InfoDetail infoDetail = db.InfoDetails.Find(id);
            if (infoDetail == null)
            {
                return HttpNotFound();
            }
            return View(infoDetail);
        }

        // POST: InfoDetail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InfoDetail infoDetail = db.InfoDetails.Find(id);
            db.InfoDetails.Remove(infoDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
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
