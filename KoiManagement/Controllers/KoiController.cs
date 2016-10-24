using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
//using KoiManagement.Models;
using KoiManagement.ViewModel;
using System.IO;
using KoiManagement.Common;
using KoiManagement.Models;
using Model.DAO;

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
        public ActionResult ListKoi(int id = 0)
        {
            if (id==0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
           //Display list koi
            var Koi = db.Kois.Where(p => p.VarietyID == id&&p.Status==true).ToList();
            return View(Koi);
        }


        // GET: /Koi/ListKoi/5
        public ActionResult KoiUser()
        {
            // Lấy KoiId theo người sở hưu
            if (Session[SessionAccount.SessionUserId] == null)
            {
               return RedirectToAction("Login", "Account");
            }
            int id = int.Parse(Session[SessionAccount.SessionUserId].ToString());
            //id = 12;
            var Owner = db.Owners.Where(p => p.OwnerID == id).ToList();
            if (Owner.Count > 0)
            {
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
            return View();

        }
        public ActionResult TestAddd()
        {
            //@@fillter by VArity id
            //var VarrityID = 2;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TestAddd(string oldPass, string newPass)
        {
            string koiname = oldPass;
            string Dob = newPass;
            //return RedirectToAction("Index", "Koi", "3");
            // su dung result
            bool result = true;
            if (result)
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

            //   return View("~/Views/koi/index.cshtml");
        }

        // GET: /Koi/Details/5
        public ActionResult Details(int id=0)
        {
            if (id == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Koi koi = db.Kois.Find(id);
                // return name of owner
            ViewBag.Owner = Common.GetOwner.GetOwnerName(id);
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
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName");
            var variety = db.Varieties.ToList();
            return View(variety);
        }

        // GET: /Koi/Create
        public ActionResult AddKoi1()
        {
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName");
            var variety = db.Varieties.ToList();
            return View(variety);
        }

        // POST: /Koi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKoi1(HttpPostedFileBase file, string KoiName, string VarietyID, string Gender, string DoB, string Temperament, string Origin)
        {
            if (ModelState.IsValid)
            {
                KoiDAO koiDao = new KoiDAO();
                //validate

                 
                // lấy id max đặt tên file ảnh
                var MaxID = koiDao.GetMaxKoiID();

                ////set default value
                //koi.Privacy = "1";
                //koi.Report = true;
                //koi.Status = true;
                //Koi = new 
                ////Save file to local
                if (file != null)
                {
                    //String id = db.Kois.SqlQuery(@"SELECT IDENT_CURRENT ('Koi') AS AnimalID").First().ToString();
                    var filename = Path.GetFileName("Koi" + MaxID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                   // koi.Image = filename;
                    var path = Path.Combine(Server.MapPath("~/Content/Image/Koi"), filename);
                    file.SaveAs(path);
                }

                //db.Kois.Add(koi);
                //db.SaveChanges();
                //// Lấy giá trị id vừa add
                //var koinewID = koi.KoiID;
                //return RedirectToAction("/ListKoi/1");
            }

            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName");
            return View();
        }

        // POST: /Koi/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddKoi(HttpPostedFileBase file, [Bind(Include = "VarietyID,KoiName,DoB,Gender,Temperament,Origin,Status")] Koi koi)
        {
            if (ModelState.IsValid)
            {
                // lấy id max đặt tên file ảnh
                var MaxID = db.Kois.Max(g => g.KoiID)+1 ;

                //set default value
                koi.Privacy = true;
                koi.Status = true;

                //Save file to local
                if (file != null)
                {
                    //String id = db.Kois.SqlQuery(@"SELECT IDENT_CURRENT ('Koi') AS AnimalID").First().ToString();
                    var filename = Path.GetFileName("Koi" + MaxID + file.FileName.Substring(file.FileName.LastIndexOf('.')));
                    koi.Image = filename;
                    var path = Path.Combine(Server.MapPath("~/Content/Image/Koi"), filename);
                    file.SaveAs(path);
                }

                db.Kois.Add(koi);
                db.SaveChanges();
                // Lấy giá trị id vừa add
                var koinewID = koi.KoiID;
                return RedirectToAction("/ListKoi/1");
            }

            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName", koi.VarietyID);
            return View(koi);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    if(System.IO.File.Exists(fullpath)){
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
