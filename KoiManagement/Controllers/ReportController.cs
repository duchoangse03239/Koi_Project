using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.DAL;
using KoiManagement.Models;

namespace KoiManagement.Controllers
{
    public class ReportController : Controller
    {
        ReportDAO reportDao=new ReportDAO();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }
        
        [HttpPost]
        public JsonResult ReportKoi(int koiId, string content)
        {
            StatusObjForJsonResult obj = new StatusObjForJsonResult();
            //check login
            if (Session[SessionAccount.SessionUserId] == null)
            {
                obj.Status = 2;
                obj.Message = "Xin hãy đăng nhập.";
                return Json(obj);
            }
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

                Report re = new Report(content, UserID,"Koi", koiId);
                if (reportDao.AddReport(re))
                {
                    obj.Status = 1;
                    obj.Message = "Bạn đã gửi báo cáo thành công.";
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