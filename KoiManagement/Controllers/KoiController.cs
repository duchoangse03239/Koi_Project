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
        KoiFarmDAO koiFarmDao = new KoiFarmDAO();
        KoiDAO koiDao = new KoiDAO();
        InfoDetailDAO DetailDao = new InfoDetailDAO();
        MemberDAO memberDao = new MemberDAO();
        CommentDAO commentDao = new CommentDAO();
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
        public ActionResult ListKoi(int? id ,int? page ,string orderby, string nameKoi, string username,string variety,string sizeFrom, string sizeTo, string gender, string owner, string AgeFrom, string AgeTo)
        {
            KoiDAO kDao = new KoiDAO();
            VarietyDAO varietyDao = new VarietyDAO();
            if (id != null&& String.IsNullOrEmpty(variety) &&string.IsNullOrEmpty(orderby))
            {
                variety = id.ToString();
            }

            if (!String.IsNullOrEmpty(variety)&&variety.Equals("0"))
            {
                variety ="";
            }
            variety = variety;
            //ViewBag.VarietyId = id;
            KoiFilterModel filter = new KoiFilterModel(orderby, nameKoi, username, variety, sizeFrom, sizeTo, gender, owner, AgeFrom, AgeTo);
            ViewBag.Filter = filter;
            ViewBag.listVariety = varietyDao.getListMainVariety();

            var koi = db.Kois.AsQueryable();

            koi = kDao.KoiFilter(filter);

            // phân trang 6 item 1trang
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Listkoi = koi.ToList().ToPagedList(pageNumber, pageSize);
            return View();
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
                MemberDAO mDAO=  new MemberDAO();
                ListKois = koiDao.GetListKoiByMember(id);
                ViewBag.Member = mDAO.GetMemberbyID(id);
                ViewBag.CountKoi = koiDao.CountKoibyOwnerId(id);
                ViewBag.CountKoiFarm = koiFarmDao.CountKoiFarmbyOwnerId(id);
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
            ViewBag.Owner = ownDao.GetOwner(id);
            // Lấy giá trị deatail cuối cùng
            var KoiDeatail = db.InfoDetails.Where(p => p.KoiID == id&&p.Status).OrderByDescending(p => p.Date);
            ViewBag.listImage =  db.Media.Where(p =>p.ModelId == KoiDeatail.FirstOrDefault().DetailID && p.Status).ToList();
            if (KoiDeatail.Any())
            {
                KoiDeatail.First();
            }
            if (koi.KoiMom != null)
            {
            ViewBag.KoiMomName = db.Kois.Find(koi.KoiMom).KoiName;
            }
            ViewBag.Size = KoiDeatail.FirstOrDefault().Size;
            ViewBag.ListComment = commentDao.GetListCommentKoi(id);
            ViewBag.ListCommentDetail = commentDao.GetListCommentKoiDetail(id);

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
        /// thêm 4 bảng koi, infodetail, Owner, media
        // POST: /Koi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public JsonResult AddKoi(string KoiName, string Size, string VarietyID, string Gender, string DoB, string Temperament, string Origin)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            DateTime? dateOfBirth = new DateTime();

            // check login
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 0;
                obj.RedirectTo = Url.Action("Login", "Account");
                return Json(obj);
            }
            string ImageKoiname = String.Empty;
            //Dùng khi thêm không thành công thì xóa
            List<string> fullpath = new List<string>();
            //List link anh
            List<Medium> ListMedia = new List<Medium>();
            string ImageDetailname = String.Empty;
            //validate
            try
            {
                if (!Validate.CheckLengthInput(KoiName, 0, 100))
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng nhập tên Koi!(Không quá 100 ký tự)";
                    return Json(obj);
                }
                if (!Validate.CheckSpecialCharacterInput(KoiName, @"^[0-9_a-zA-Z_ÀÁÂÃÈÉÊÌÍÒÓÔÕÙÚĂĐĨŨƠàáâãèéêìíòóôõùúăđĩũơƯĂẠẢẤẦẨẪẬẮẰẲẴẶẸẺẼỀỀỂưăạảấầẩẫậắằẳẵặẹẻẽềềểỄỆỈỊỌỎỐỒỔỖỘỚỜỞỠỢỤỦỨỪễệỉịọỏốồổỗộớờởỡợụủứừỬỮỰỲỴÝỶỸửữựỳỵỷỹ]+$/"))
                {
                    obj.Status = 2;
                    obj.Message = "Vui lòng không nhập ký tự đặc biệt cho tên Koi";
                    return Json(obj);
                }

                if (String.IsNullOrWhiteSpace(Size))
                {
                    obj.Status = 3;
                    obj.Message = "Kích thước không được để trống";
                    return Json(obj);
                }
                if (!Validate.CheckIsDouble(Size))
                {
                    obj.Status = 3;
                    obj.Message = "Vui lòng nhập kiểu số cho kích thước của Koi";
                    return Json(obj);
                }
                if (!Validate.ValidateDate(DoB))
                {
                    obj.Status = 4;
                    obj.Message = "Xin hãy nhập đúng định dạng ngày sinh";
                    return Json(obj);
                }
                else
                {
                    dateOfBirth = Validate.ConverDateTime(DoB);
                }

                //Lấy file ảnh
                HttpFileCollectionBase files = Request.Files;
                HttpPostedFileBase file = null;

                decimal size;
                size = decimal.Parse(Size);
                DateTime? dateOfBirth1 = Validate.ConverDateTime(DoB);
                // lấy id max đặt tên file ảnh
                var MaxKoiID = koiDao.GetMaxKoiID();
                var MaxDetailID = DetailDao.GetMaxDetailID();
                var koi = new Koi(int.Parse(VarietyID), KoiName, dateOfBirth, Gender, Temperament, DoB, "certificate",
                    Origin, 1, true);

                //Trường hợp có nhiều file ảnh
                for (int i = 0; i < files.Count; i++)
                {
                    file = files[i];
                    //Save file to local
                    //file đầu tiên làm avartar
                    if (file != null && i == 0)
                    {
                        //thêm ảnh cho koi
                        ImageKoiname = Path.GetFileName("Koi" + MaxKoiID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        koi.Image = ImageKoiname;
                        fullpath.Add(Server.MapPath("~/Content/Image/Koi/" + ImageKoiname));
                        var pathKoi = Path.Combine(Server.MapPath("~/Content/Image/Koi"), ImageKoiname);
                        file.SaveAs(pathKoi);
                        //thêm ảnh cho koi detail
                        ImageDetailname = Path.GetFileName("Detail" + MaxDetailID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        
                        fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + ImageDetailname));
                        var pathDetail = Path.Combine(Server.MapPath("~/Content/Image/Detail"), ImageDetailname);
                        file.SaveAs(pathDetail);
                    }
                    else if (file != null)
                    {
                        var filename = Path.GetFileName("Detail" + MaxDetailID+ "."+ i + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        fullpath.Add(Server.MapPath("~/Content/Image/Detail/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Detail"), filename);
                        file.SaveAs(path);
                        Medium a = new Medium();
                        a.LinkImage = filename;
                        a.Status = true;
                        ListMedia.Add(a);
                    }
                }

                // thêm thông tin koi vào 3 bảng: koi, owner, infoDetail
                if (koiDao.AddKoi(koi, int.Parse(Session[SessionAccount.SessionUserId].ToString()), ImageDetailname, size, ListMedia))
                {
                    //success
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm Koi thành công";
                    obj.RedirectTo = Url.Action("KoiUser/" + Session[SessionAccount.SessionUserId], "Koi");
                    return Json(obj);
                }
                else
                {
                    //Nếu fail xoa anh da them
                    foreach (var image in fullpath)
                    {
                        if (System.IO.File.Exists(image))
                        {
                            System.IO.File.Delete(image);
                        }
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
                //Nếu fail xoa anh da them
                foreach (var image in fullpath)
                {
                    if (System.IO.File.Exists(image))
                    {
                        System.IO.File.Delete(image);
                    }
                }
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
                Koi koi = new Koi(int.Parse(VarietyID),KoiName, dateOfBirth, Gender,Temperament,DoB,"",Origin,1,true);
                koi.KoiID = int.Parse(KoiId);
                koi.Image = Image;
                //Edit file to local
                if (file != null)
                {
                    if (string.IsNullOrWhiteSpace(koi.Image))
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
            koi.Status = -1;
            db.Kois.Attach(koi);
            db.Entry(koi).Property(x => x.Status).IsModified = true;
            int result= db.SaveChanges();
            //return View();
            if (result==1)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        [HttpPost]
        public JsonResult ChangeOwner(string koiId,string username)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            int UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            try
            {
                OwnerDAO ownerDao = new OwnerDAO();
            if (string.IsNullOrWhiteSpace(username))
            {
                obj.Status = 2;
                obj.Message = "Xin hãy nhập tên đăng nhập";
                return Json(obj);
            }
            if (memberDao.CheckExistUserName(username))
            {
                obj.Status = 3;
                obj.Message = "Tên đăng nhập không tồn tại";
                return Json(obj);
            }
                var koiID = int.Parse(koiId);
                var NewMemberID = memberDao.getExistUserName(username);
                var mem = memberDao.GetMemberbyID(UserID);
                var koiName = db.Kois.Find(koiID).KoiName;

                Notification notification = new Notification(NewMemberID.MemberID,UserID , 1,koiID, DateTime.Now
                    , mem.Name +" muốn chuyển nhượng "+ koiName+" cho bạn","", false,true);

                NotificationDAO noDao = new NotificationDAO();
            if (noDao.AddNotification(notification))
            {
                obj.Status = 1;
                ViewBag.NewMemberID = NewMemberID;
                obj.Message = "Bạn đã đổi sang chủ "+ username+" thành công vui lòng chờ xác nhận";
                obj.JsonObject = NewMemberID.MemberID;
                   // ownerDao.ChangeOwner(username, int.Parse(koiId))
                return Json(obj);
            }

            }catch (Exception ex)
            {
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }
        [HttpPost]
        public JsonResult SentMessage(int koiId, string content,int ToMember)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            // check login
            //if (Session[SessionAccount.SessionUserId] == null)
            //{
            //    obj.Status = 2;
            //    obj.Message = "Xin hãy đăng nhập.";
            //    return Json(obj);
            //}
            int UserID = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            try
            {

                OwnerDAO ownerDao = new OwnerDAO();
                if (string.IsNullOrWhiteSpace(content))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập tên nội dung.";
                    return Json(obj);
                }

                var mem = memberDao.GetMemberbyID(UserID);
                var koiName = db.Kois.Find(koiId).KoiName;

                Notification notification = new Notification(ToMember, UserID,1,koiId,DateTime.Now,content,"",false,true);

                NotificationDAO noDao = new NotificationDAO();
                if (noDao.AddNotification(notification))
                {
                    obj.Status = 1;
                    obj.Message = "Bạn đã gửi tin nhắn đến "+ mem.Name +"thành công.";
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
        public JsonResult ChangeOwnerConfirm(int NotID, int userid, string koiId)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            OwnerDAO ownerDao = new OwnerDAO();

            if (ownerDao.ChangeOwner(NotID, userid, int.Parse(koiId)))
            {
                obj.Status = 1;
                obj.Message = "Bạn đã nhận thành công";
                // ownerDao.ChangeOwner(username, int.Parse(koiId))
                return Json(obj);
            }
            return Json(obj);
        }

        [HttpPost]
        public JsonResult ToDead(string KoiId, string DeadReason)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();

            try
            {
                if (string.IsNullOrWhiteSpace(DeadReason))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập lý do khai tử";
                    return Json(obj);
                }

                if (koiDao.ToDead(int.Parse(KoiId), DeadReason)>0)
                {
                    obj.Status = 1;
                    obj.Message = "Bạn đã khai tử thành công";
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
        public JsonResult Rating(int koiID,string RateNum,string content)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();

            try
            {
                if (Session[SessionAccount.SessionUserId] == null)
                {
                    obj.Status =2;
                    obj.Message = "Xin hãy đăng nhập để đánh giá.";
                    return Json(obj);
                }
                if (string.IsNullOrWhiteSpace(content))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy nhập nội dung đánh giá.";
                    return Json(obj);
                }
                if (!Validate.CheckIsDouble(RateNum))
                {
                    obj.Status = 2;
                    obj.Message = "Xin hãy chọn sao đánh giá.";
                    return Json(obj);
                }
                decimal sao = decimal.Parse(RateNum);
                Comment c = new Comment(int.Parse(Session[SessionAccount.SessionUserId].ToString()), koiID,DateTime.Now, null,sao,content,null,true);
                if (commentDao.addComment(c))
                {
                    obj.Status = 1;
                    obj.Message = "Bạn đã gửi đánh giá thành công!";
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

    }
}
