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
        KoiDAO koiDao = new KoiDAO();
        OwnerDAO ownerDao = new OwnerDAO();
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
                return RedirectToAction("PageNotFound", "Error");
            }
            var infoDetail = db.InfoDetails.Where(p => p.KoiID == id&&p.Status).OrderByDescending(p => p.Date);
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
                ViewBag.listMonth = db.InfoDetails.Where(p => p.KoiID == id && p.Status&&p.Date.Year == year).OrderBy(p => p.Date).ToList();
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
            try
            {
                List<List<Medium>> myList = new List<List<Medium>>();
            string t = "";
            BaseFilter filter;
            pageNum = pageNum ?? 1;
            filter = new BaseFilter { CurrentPage = pageNum.Value };



            var ListInfo = db.InfoDetails.Where(p => p.KoiID == id&&p.Status).OrderByDescending(p => p.Date).Skip(filter.Skip).Take(filter.ItemsPerPage).ToList();
            // if (ListInfo.Count < filter.ItemsPerPage) ViewBag.IsEndOfRecords = true;

            List<String> ListImage = new List<string>();
            foreach (var item in ListInfo)
            {
                var ListImage1 = db.Media.Where(p =>  p.ModelId == item.DetailID&&p.Status).ToList();
                ListImage1.Add(new Medium(item.Image, "", item.DetailID,true));
                myList.Add(ListImage1);
            }
            foreach (List<Medium> subList in myList)
            {
                t = string.Empty;
                foreach (var item in subList)
                {
                    t = t + "\'" + @Url.Content("~/Content/Image/Detail/" + item.LinkImage.Trim()) + "\',\n";
                }
                ListImage.Add(t);
            }
            ViewBag.ListInfo1 = ListInfo;
            ViewBag.ListAnh1 = ListImage;
            ViewBag.koiId = id;
            ViewBag.Skip = filter.Skip;

            return View();
            }
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                return RedirectToAction("SystemError", "Error");
            }
        }
        public ActionResult timeline(int? id, int? pageNum, int? filterVal)
        {
            try
            {
                //check id null
                List<List<Medium>> myList = new List<List<Medium>>();
            string t = "";
            BaseFilter filter;
            pageNum = pageNum ?? 1;
            filter = new BaseFilter { CurrentPage = pageNum.Value };



            var ListInfo = db.InfoDetails.Where(p => p.KoiID == id&&p.Status).OrderByDescending(p => p.Date).Skip(filter.Skip).Take(filter.ItemsPerPage).ToList();
            if (ListInfo.Count < filter.ItemsPerPage) ViewBag.IsEndOfRecords = true;

            List<String> ListImage = new List<string>();
            foreach (var item in ListInfo)
            {
                var ListImage1 = db.Media.Where(p => p.ModelId == item.DetailID && p.Status).ToList();
                ListImage1.Add(new Medium(item.Image,"",item.DetailID,true));
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
            catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                return RedirectToAction("SystemError", "Error");
            }
        }

        // GET: InfoDetail/add new koi
        public ActionResult AddDetail(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            var koi = koiDao.getKoiById(id.Value);
            if (koi == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ViewBag.KoiID = id.Value;
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
                    obj.Message = "Vui lòng không nhập số âm cho kích thước của Koi!";
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
                        var filename = Path.GetFileName("Detail" + MaxID + "." + i + file.FileName.Substring(file.FileName.LastIndexOf('.')) );
                        infoDetail.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
                        file.SaveAs(path);
                    }
                    else if (file != null )
                    {
                        var filename = Path.GetFileName("Detail" + MaxID + "." + i + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        infoDetail.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
                        file.SaveAs(path);
                        Medium a =  new Medium();
                        a.LinkImage = filename;
                        a.Status = true;
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
                    obj.Message = "Bạn đã thêm thành công.";
                    obj.RedirectTo = Url.Action("timeline/" + KoiID, "InfoDetail");
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
            SessionInfoDetail.listRemoveImage.Clear();
            if (id == null)
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            InfoDetail infoDetail = db.InfoDetails.Find(id);
            if (infoDetail == null )
            {
                return RedirectToAction("PageNotFound", "Error");
            }
            ViewBag.KoiID = infoDetail.KoiID;
            List<Medium> listImage = new List<Medium>();
            listImage.Add(new Medium(infoDetail.Image,"", id.Value,true));
            var a = db.Media.Where(p =>  p.ModelId == id && p.Status).ToList();

            listImage.AddRange(a);

            ViewBag.ListImage = listImage;
            return View(infoDetail);
        }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult EditDetail(string KoiID,int DetailID, string Size, string HeathyStatus, string Description, string Avatar)
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
                if (double.Parse(Size) < 0)
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng không nhập số âm cho kích thước của Koi!";
                    return Json(obj);
                }
                var listImageRemove = SessionInfoDetail.listRemoveImage;
                InfoDetailDAO infoDetailDao = new InfoDetailDAO();
                MediaDAO mediaDao = new MediaDAO();
                var infoDetail = infoDetailDao.GetDetailByid(DetailID);
                infoDetail.Size = decimal.Parse(Size);
                infoDetail.HeathyStatus = HeathyStatus;
                infoDetail.Description = Description;
                infoDetail.KoiID = int.Parse(KoiID);
                infoDetail.Status = true;
                //xóa ảnh
                foreach (var item in listImageRemove)
                {
                    if (item != null&&item.LinkImage!=null) { 
                    var path = Server.MapPath("~/Content/Image/Detail/" + item.LinkImage);
                    if (System.IO.File.Exists(Server.MapPath("~/Content/Image/Detail/" + item.LinkImage)))
                    {
                        System.IO.File.Delete(path);
                    }
                    }
                }


                // lấy id đặt tên
                double MaxID = DetailID + (double)db.Media.Count(p => p.ModelId == DetailID)/10 - (double)listImageRemove.Count/10;

                //Lấy file ảnh
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = null;
                //Trường hợp chỉ  có nhiều file ảnh
                for (int i = 0; i < files.Count; i++)
                {
                    //string filename = Path.GetFileName(Request.Files[i].FileName);  
                    file = files[i];
                    if (file != null)
                    {
                    var name = file.FileName;
                    //Save file to local
                    if (name.Equals(Avatar))
                    {
                            Medium a = new Medium();
                            a.LinkImage = infoDetail.Image;
                            a.Status = true;
                            lishMedia.Add(a);

                            var filename =
                            Path.GetFileName("Detail" + MaxID + "."+ i +
                                             file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        infoDetail.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
                        file.SaveAs(path);
        
                        }
                    else 
                    {
                        var filename =
                            Path.GetFileName("Detail" + MaxID + "." + i +
                                             file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        infoDetail.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
                        file.SaveAs(path);
                        Medium a = new Medium();
                        a.LinkImage = filename;
                        a.Status = true;
                        lishMedia.Add(a);
                    }
                    }
                }



                    if (infoDetailDao.UpdateDetail(infoDetail, listImageRemove, lishMedia, Avatar))
                {

                    obj.Status = 1;
                    obj.RedirectTo = Url.Action("EditDetail/"+DetailID, "InfoDetail");
                    obj.Message = "Bạn đã cập nhật thành công.";
                    return Json(obj);
                }
                else
                {
                    foreach (var item in fullpath)
                    {
                        if (System.IO.File.Exists(item))
                        {
                            System.IO.File.Delete(item);
                        }
                    }
                    obj.Status = 1;
                    obj.Message = "Bạn đã cập nhật thành công.";
                    return Json(obj);
                }


                //update bang media
                ViewBag.KoiID = new SelectList(db.Kois, "KoiID", "KoiName", infoDetail.KoiID);
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

        [HttpPost]
        public JsonResult ListImageDelete(int ModelID)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            var Model = db.Media.Find(ModelID);
            SessionInfoDetail.listRemoveImage.Add(Model);
            return Json(obj);
        }


        // POST: InfoDetail/Delete/5
        [HttpPost]
        public ActionResult DeleteConfirmed(int Detailid)
        {
            InfoDetail koi = db.InfoDetails.Find(Detailid);
            koi.Status = false;
            db.InfoDetails.Attach(koi);
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

        public ActionResult ParentTree(int? id)
        {
            if (id == null)
            {
                RedirectToAction("PageNotFound", "Error");
            }
            var koimom = db.Kois.Find(id);
            if (koimom == null)
            {
                RedirectToAction("PageNotFound", "Error");
            }
            List<Koi> listKoiSon = koiDao.getListKoiSonByKoiID(id.Value);
            string t = string.Empty;
            //thêm koi mẹ
            if (koimom.KoiMom.HasValue)
            {
                Koi KOI = koiDao.getKoiById(koimom.KoiMom.Value);
                t = t + " { key: \'" + KOI.KoiID + "\', name: \'" + KOI.KoiName + "\', source: \'" + @Url.Content("~/Content/Image/Koi/" + KOI.Image) + "\'},\n";
              var  koison2 = koiDao.getKoiById(id.Value);
                t = t + " { key: \'" + koison2.KoiID + "\',parent: \'" + KOI.KoiID + "\', name: \'" + koison2.KoiName + "\', source: \'" + @Url.Content("~/Content/Image/Koi/" + koison2.Image) + "\', highlight: true},\n";
            }
            else
            {
                Koi KOI = koiDao.getKoiById(id.Value);
                t = t + " { key: \'" + KOI.KoiID + "\',parent: \'" + KOI.KoiID + "\', name: \'" + KOI.KoiName + "\', source: \'" + @Url.Content("~/Content/Image/Koi/" + KOI.Image) + "\', highlight: true},\n";
            }
          
                foreach (var item in listKoiSon)
                {
                t = t + " { key: \'" + item.KoiID + "\',parent: \'" + id.Value + "\', name: \'" + item.KoiName + "\', source: \'" + @Url.Content("~/Content/Image/Koi/" + item.Image) + "\'},\n";

                 }
             ViewBag.allKoi = t;
            ViewBag.koiId = id;
            return View();
        }
        public ActionResult AddParent(int id, int? page, string nameKoi, string variety, string owner)
        {
            KoiDAO kDao = new KoiDAO();
            //if (!String.IsNullOrEmpty(variety) &&variety.Equals("0"))
            //{
            //    variety = "";
            //}
            //else if (String.IsNullOrEmpty(variety))
            //{
            //    variety = "0";
            //}
            if (!String.IsNullOrEmpty(variety) && variety.Equals("0"))
            {
                variety = "";
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
                if (koiSonId == koMomId)
                {
                    obj.Status = 2;
                    obj.Message = "Bạn đã không thể thêm chính koi đó làm mẹ.";
                    return Json(obj);
                }
                if (kDao.AddParent(koiSonId, koMomId) >0)
                {
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm mẹ cho cá Koi thành công";
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

        public ActionResult AllOWner(int? id)
        {
            if (id == null)
            {
                RedirectToAction("PageNotFound", "Error");
            }
            var koimom = db.Kois.Find(id);
            if (koimom == null)
            {
                RedirectToAction("PageNotFound", "Error");
            }
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            ViewBag.ListOwner = ownerDao.GetAllOwnersByKoiID(id.Value);
            ViewBag.koiId = id;
            ViewBag.Koi = koimom;
            return View();
        }
        [HttpPost]
        public ActionResult AddNewParent(int koiSonId, string koiname, string VarietyId, string Origin)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            KoiDAO koiDao = new KoiDAO();
            try
            {
                if (String.IsNullOrWhiteSpace(koiname))
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng nhập tên Koi";
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
                var koi = new Koi(int.Parse(VarietyId), koiname, null, null, "", "", "",
                     Origin, 1, true);

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
                    obj.Message = "Bạn đã thêm mẹ cho cá Koi thành công";
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
