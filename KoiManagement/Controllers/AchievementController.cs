using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace KoiManagement.Controllers
{
    public class AchievementController : Controller
    {
        /// <summary>
        /// Add Achievemnt
        /// </summary>
        /// <returns>View</returns>
        public ActionResult AddAchievement()
        {
            return View();
        }

        // GET: /Account/AddAchievement
        [HttpPost]
        public JsonResult AddAchievement(int? id, string time, string description)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            //check login
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 0;
                obj.RedirectTo = Url.Action("Login", "Account");
                return Json(obj);
            }
            string Imagename = String.Empty;
            var fullpath = new List<string>();

            //Khởi tạo giá trị cho trường không bắt buộc
            DateTime? timeAchievement = new DateTime();

            //validate
            try
            {
                if (string.IsNullOrWhiteSpace(time) && Validate.ValidateDate(time))
                {
                    obj.Status = 2;
                    obj.Message = "Ngày tháng không đúng định dạng!";
                    return Json(obj);
                }
                else
                {
                    timeAchievement = Validate.ConverDateTime(time);
                }

                AchievementDAO AchiDao = new AchievementDAO();
                Achievement ad = new Achievement();
                // lấy id max đặt tên file ảnh
                var MaxAchiID = AchiDao.GetMaxAchiID();
                ad.Date = timeAchievement;
                ad.Description = description;
                ad.Status = true;
                ad.KoiID = id;//@hardcode


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
                        var filename = Path.GetFileName("AchiKoi"+ ad.KoiID + MaxAchiID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        ad.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Achievement/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Achievement"), filename);
                        file.SaveAs(path);
                    }
                    else if (file != null)
                    {
                        var filename = Path.GetFileName("AchiKoi" + ad.KoiID + MaxAchiID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        ad.Image = filename;
                        fullpath.Add(Server.MapPath("~/Content/Image/Achievement/" + filename));
                        var path = Path.Combine(Server.MapPath("~/Content/Image/Achievement"), filename);
                        file.SaveAs(path);
                    }
                }

                if (AchiDao.AddAchievement(ad))
                {
                    //success
                    obj.Status = 1;
                    obj.Message = "Bạn đã thêm giải thưởng cho cá thành công";
                    obj.RedirectTo = Url.Action("ListAchievement/" + ad.KoiID, "Achievement");
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

        /// <summary>
        /// Edit Achievemnt
        /// </summary>
        /// <returns>View</returns>
        public ActionResult EditAchievement()
        {
            return View();
        }

        // GET: /Account/UpdateProfile
        [HttpPost]
        public JsonResult EditAchievement(int id,string image, string time, string description)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            // check login
            //if (Session[SessionAccount.SessionUserId] == null)
            //{
            //    obj.Status = 0;
            //    obj.RedirectTo = Url.Action("Login", "Account");
            //    return Json(obj);
            //}
            try{
            AchievementDAO AchiDao = new AchievementDAO();
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
                DateTime? timeAchievement = Validate.ConverDateTime(time);
                Achievement achi = new Achievement();
                achi.Image = image;
                achi.KoiID = id;
                achi.Date = timeAchievement;
                achi.Description = description;
                achi.Status = true;
                //Edit file to local
                if (file != null)
                {
                    if (achi.Image == null)
                    {
                        filename = Path.GetFileName("Achi" + achi.KoiID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                        achi.Image = filename;
                    }
                    else
                    {
                        filename = achi.Image;
                    }
                    var fullpath = Server.MapPath("~/Content/Image/Achievement/" + filename);
                    var path = Path.Combine(Server.MapPath("~/Content/Image/Achievement"), filename);
                    if (System.IO.File.Exists(fullpath))
                    {
                        System.IO.File.Delete(fullpath);
                    }
                    file.SaveAs(path);
                }
                //thanh cong
                if (AchiDao.EditAchievement(achi) == 1)
                {
                    obj.Status = 1;
                    obj.Message = "Cập nhật thông tin thành công";
                    obj.RedirectTo = Url.Action("KoiUser/" + Session[SessionAccount.SessionUserId], "Koi");
                }
            } 
            catch (Exception ex){
                Common.Logger.LogException(ex);
                obj.Status = 0;
                obj.RedirectTo = this.Url.Action("SystemError", "Error");
                return Json(obj);
            }
            return Json(obj);
        }

        public ActionResult ListAchivement(int? id)
        {
            AchievementDAO AchiDao = new AchievementDAO();
            if (id == null)
            {
                return View();
            }
            ViewBag.listIAchi = AchiDao.GetListAchievements(id.Value);
            ViewBag.koiId = id;
            return View();

        }
        private KoiManagementEntities db = new KoiManagementEntities();
        [HttpPost]
        public ActionResult DeleteConfirmed(int AchiId)
        {
            Achievement achie = db.Achievements.Find(AchiId);
            achie.Status = false;
            db.Achievements.Attach(achie);
            db.Entry(achie).Property(x => x.Status).IsModified = true;
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