using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using KoiManagement.DAL;
using KoiManagement.Models;

//using KoiManagement.Models;

namespace KoiManagement.Controllers
{
    public class VarietyController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();

        // GET: /Variety/
        public ActionResult Index()
        {
            
            var varieties = db.Varieties.Include(v => v.Variety1).Include(v => v.Variety2);
            return View(varieties.ToList());
        }

        // GET: /List Variety/
        public ActionResult ListVariety()
        {
            VarietyDAO varietyDao = new VarietyDAO();
            ViewBag.Varieties = varietyDao.getListMainVariety();
            return View();
        }

        // GET: /Variety/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Variety variety = db.Varieties.Find(id);
            if (variety == null)
            {
                return HttpNotFound();
            }
            return View(variety);
        }

        // GET: /Variety/Create
        public ActionResult Create()
        {
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName");
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName");
            return View();
        }

        // POST: /Variety/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="VarietyID,VarietyName,Description,Image,VarietyDetailID")] Variety variety)
        {
            if (ModelState.IsValid)
            {
                db.Varieties.Add(variety);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName", variety.VarietyID);
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName", variety.VarietyID);
            return View(variety);
        }

        // GET: /Variety/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Variety variety = db.Varieties.Find(id);
            if (variety == null)
            {
                return HttpNotFound();
            }
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName", variety.VarietyID);
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName", variety.VarietyID);
            return View(variety);
        }

        // POST: /Variety/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="VarietyID,VarietyName,Description,Image,VarietyDetailID")] Variety variety)
        {
            if (ModelState.IsValid)
            {
                db.Entry(variety).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName", variety.VarietyID);
            ViewBag.VarietyID = new SelectList(db.Varieties, "VarietyID", "VarietyName", variety.VarietyID);
            return View(variety);
        }

        // GET: /Variety/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Variety variety = db.Varieties.Find(id);
            if (variety == null)
            {
                return HttpNotFound();
            }
            return View(variety);
        }

        // POST: /Variety/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Variety variety = db.Varieties.Find(id);
            db.Varieties.Remove(variety);
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
