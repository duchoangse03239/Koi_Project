using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using PagedList;

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

 

        public ActionResult TimelineVertical(int? id, int? year)
        {
            //check id null
            List<InfoDetail> listYear = new List<InfoDetail>();
            if (year == null)
            {
                //loc theo năm
                listYear =
                    db.InfoDetails.Where(p => p.KoiID == id&&p.Status)
                        .OrderBy(p => p.Date)
                        .GroupBy(p => p.Date.Year)
                        .Select(p => p.FirstOrDefault())
                        .ToList();
            }
            else
            {
                //lọc theo tháng và ngày
                listYear = db.InfoDetails.Where(p => p.KoiID == id && p.Status&&p.Date.Year == year).OrderBy(p => p.Date).ToList();
            }
            //theo tháng
            //var test1 = db.InfoDetails.Where(p1 => p1.Date.Year == 2015).GroupBy(p => p.Date.Month).Select(p => p.FirstOrDefault()).ToList();
            //theo tung ngày
            //var test2 = db.InfoDetails.Where(p => p.Date.Year == 2015&&p.Date.Month==12).ToList();
            ViewBag.listYear = listYear;
            ViewBag.koiId = id;
            ViewBag.year = year;
            return View();
        }
        public ActionResult UpdateTimeLine(int? id, int? pageNum, int? filterVa)
        {
            List<List<Medium>> myList = new List<List<Medium>>();
            string t = "";
            BaseFilter filter;
            pageNum = pageNum ?? 1;
            filter = new BaseFilter { CurrentPage = pageNum.Value };



            var ListInfo = db.InfoDetails.Where(p => p.KoiID == id).OrderByDescending(p => p.Date).Skip(filter.Skip).Take(filter.ItemsPerPage).ToList();
            // if (ListInfo.Count < filter.ItemsPerPage) ViewBag.IsEndOfRecords = true;

            List<String> ListImage = new List<string>();
            foreach (var item in ListInfo)
            {
                var ListImage1 = db.Media.Where(p => p.ModelTypeID == "infodetail" && p.ModelId == item.DetailID).ToList();
                ListImage1.Add(new Medium(item.Image, "", item.DetailID, "infodetail"));
                myList.Add(ListImage1);
            }
            foreach (List<Medium> subList in myList)
            {
                t = string.Empty;
                foreach (var item in subList)
                {
                    t = t + "\'" + @Url.Content("~/Content/Image/Detail/" + item.LinkImage) + "\',\n";
                }
                ListImage.Add(t);
            }
            ViewBag.ListInfo1 = ListInfo;
            ViewBag.ListAnh1 = ListImage;
            ViewBag.koiId = id;
            ViewBag.Skip = filter.Skip;

            return View();
        }
        public ActionResult timeline(int? id, int? pageNum, int? filterVal)
        {
            //check id null
            List<List<Medium>> myList = new List<List<Medium>>();
            string t = "";
            BaseFilter filter;
            pageNum = pageNum ?? 1;
            filter = new BaseFilter { CurrentPage = pageNum.Value };



            var ListInfo = db.InfoDetails.Where(p => p.KoiID == id).OrderByDescending(p => p.Date).Skip(filter.Skip).Take(filter.ItemsPerPage).ToList();
            if (ListInfo.Count < filter.ItemsPerPage) ViewBag.IsEndOfRecords = true;

            List<String> ListImage = new List<string>();
            foreach (var item in ListInfo)
            {
                var ListImage1 = db.Media.Where(p => p.ModelTypeID == "infodetail" && p.ModelId == item.DetailID).ToList();
                ListImage1.Add(new Medium(item.Image,"",item.DetailID, "infodetail"));
                myList.Add(ListImage1);
            }
            foreach (List<Medium> subList in myList)
            {
                t =string.Empty;
                foreach (var item in subList)
                {
                    t = t + "\'" + @Url.Content("~/Content/Image/Detail/" + item.LinkImage) + "\',\n";
                }
                ListImage.Add(t);
            }
            ViewBag.ListInfo = ListInfo;
            ViewBag.ListAnh = ListImage;
            ViewBag.koiId = id;

            return View();
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
        [HttpPost]
        public JsonResult AddDetail(string KoiID,  string Size, string HeathyStatus, string Description)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            var fullpath = new List<string>();
            var lishMedia = new List<Medium>();
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 0;
                obj.RedirectTo = Url.Action("Login", "Account");
                return Json(obj);
            }
            try
            {
                if (String.IsNullOrWhiteSpace(Size))
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng nhập kích thước!";
                    return Json(obj);
                }
                if (!Validate.CheckIsDouble(Size))
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng nhập kiểu số cho kích thước!";
                    return Json(obj);
                }
                if (double.Parse(Size)<0)
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng nhập số cho kích thước của Koi!";
                    return Json(obj);
                }

                InfoDetailDAO infoDetailDao = new InfoDetailDAO();
                MediaDAO mediaDao = new MediaDAO();
                var infoDetail = new InfoDetail();
                
                infoDetail.Size = decimal.Parse(Size);
                infoDetail.HeathyStatus = HeathyStatus;
                infoDetail.Description = Description;
                infoDetail.KoiID = int.Parse(KoiID);
                infoDetail.Status = true;
                // lấy id max đặt tên file ảnh
                var MaxID = infoDetailDao.GetMaxDetailID();

                //Lấy file ảnh
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = null;
                //Trường hợp chỉ  có nhiều file ảnh
                for (int i = 0; i < files.Count; i++)
                {
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  
                    file = files[i];
                    //Save file to local
                    //file đầu tiên làm avartar
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
                        lishMedia.Add(a);
                    }
                }
                //set default value
                infoDetail.Date = DateTime.Now;

                // return RedirectToAction("KoiInfoDetail/"+infoDetail.KoiID);

                ViewBag.KoiID = new SelectList(db.Kois, "KoiID", "KoiName", infoDetail.KoiID);
                 if (infoDetailDao.AddInfoDetail(infoDetail, lishMedia))
                {
                    //success
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm";
                    obj.RedirectTo = Url.Action("timeline", "InfoDetail");
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

        public ActionResult EditDetail(int? id)
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDetail(HttpPostedFileBase file, [Bind(Include = "DetailID,KoiID,Weight,Image,Size,HeavyStatus,Description")] InfoDetail infoDetail)
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
        public ActionResult AddParent(int id, int? page, string nameKoi, string variety, string owner)
        {
            KoiDAO kDao = new KoiDAO();
            if (!String.IsNullOrEmpty(variety) &&variety.Equals("0"))
            {
                variety = "";
            }
            else if (String.IsNullOrEmpty(variety))
            {
                variety = "0";
            }
            KoiFilterModel filter = new KoiFilterModel("", nameKoi, "", variety, "", "", "", owner, "", "");
            ViewBag.Filter = filter;
            var koi = db.Kois.AsQueryable();


            koi = kDao.KoiFilter(filter);
            VarietyDAO varietyDao= new VarietyDAO();
            ViewBag.listVariety = varietyDao.getListMainVariety();

            koi = kDao.KoiFilter(filter);

            ViewBag.KoiSonID = id;
            // phân trang 6 item 1trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Listkoi = koi.ToList().ToPagedList(pageNumber, pageSize);

            return View();
        }
        [HttpPost]
        public ActionResult AddToParent(int koiSonId, int koMomId)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            KoiDAO kDao = new KoiDAO();
            try
            {
                if (kDao.AddParent(koiSonId, koMomId) >0)
                {
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm mẹ thành công";
                    obj.RedirectTo = Url.Action("Details/" + koiSonId, "Koi");
                    return Json(obj);
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
        public ActionResult AddNewParent(int koiSonId, string koiname,string Gender, string VarietyId, string Origin)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            KoiDAO koiDao = new KoiDAO();
            try
            {
                if (String.IsNullOrWhiteSpace(koiname))
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng nhập tên koi";
                    return Json(obj);
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
                string fullpath="";
                string ImageKoiname;
                var MaxKoiID = koiDao.GetMaxKoiID();
                var koi = new Koi(int.Parse(VarietyId), koiname, null, Gender, "", "", "",
                     Origin, true, true);

                if (file != null)
                {
                    ImageKoiname = Path.GetFileName("Koi" + MaxKoiID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                    koi.Image = ImageKoiname;
                    fullpath = Server.MapPath("~/Content/Image/Koi/" + ImageKoiname);
                    var pathKoi = Path.Combine(Server.MapPath("~/Content/Image/Koi"), ImageKoiname);
                    file.SaveAs(pathKoi);
                }
                if ( koiDao.AddNewParent(koi,koiSonId))
                {
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm mẹ thành công";
                    obj.RedirectTo = Url.Action("Details/" + koiSonId, "Koi");
                    return Json(obj);
                }
                else
                {
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
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
