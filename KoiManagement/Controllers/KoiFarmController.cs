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
            var listkoi = koiFarmDao.GetListKoiByKoiFarmId((int)id);
            var koifarm = koiFarmDao.GetKoiFarmDetail((int)id);
            ViewBag.koiFarm = koifarm;
            ViewBag.listKoi = listkoi;
            return View();
        }

        public ActionResult CreateKoiFarm()
        {
            return View();
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult CreateKoiFarm(string farmName, string address,string description)
        {
            try { 
            KoiFarm  koiFarm = new KoiFarm();
            koiFarm.FarmName = farmName;
            koiFarm.Address = address;
            koiFarm.Description = description;
            koiFarm.Status = true;
            db.KoiFarms.Add(koiFarm);
            db.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var entityValidationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in entityValidationErrors.ValidationErrors)
                    {
                        Response.Write("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
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
                var db1 = db.Kois.Where(p => p.Owners.Where(o => o.Status).FirstOrDefault().KoiFarmID == koifarm.KoifarmID);
                foreach (var item in db1)
                {
                    if (!dict.ContainsKey(item.Variety.VarietyID) && !item.Variety.VarietyName.Equals("Chưa rõ"))
                        dict.Add(item.Variety.VarietyID, item.Variety.VarietyName);
                }
                foreach (var v in dict)
                {
                    t += v.Value +", ";
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

    }
}