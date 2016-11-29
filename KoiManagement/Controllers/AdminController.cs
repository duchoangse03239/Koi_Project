using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Models;
using PagedList;

namespace KoiManagement.Controllers
{
    public class AdminController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ListMember(string sortOrder, string currentFilter, string searchString, int? page)
        {
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.UserName = String.IsNullOrEmpty(sortOrder) ? "username_desc" : "";
            ViewBag.Name = sortOrder == "Name" ? "Name_desc" : "Name";
            ViewBag.Phone = sortOrder == "Phone" ? "Phone_desc" : "Phone";
            ViewBag.Email = sortOrder == "Email" ? "Email_desc" : "Phone";

            ViewBag.CurrentFilter = searchString;

            var Member = db.Members.AsQueryable();
            Member = Member.OrderBy(p=>p.Name);
            //search
            if (!String.IsNullOrEmpty(searchString))
            {
                Member = Member.Where(s => s.UserName.Contains(searchString)|| s.Name.Contains(searchString) || s.Phone.Contains(searchString)|| s.Email.Contains(searchString));
            }
            //sort
            switch (sortOrder)
            {
                case "name_desc":
                    Member = Member.OrderByDescending(s => s.UserName);
                    break;
                case "Name":
                    Member = Member.OrderBy(s => s.Name);
                    break;
                case "Name_desc":
                    Member = Member.OrderByDescending(s => s.Name);
                    break;
                case "Phone":
                    Member = Member.OrderBy(s => s.Phone);
                    break;
                case "Phone_desc":
                    Member = Member.OrderByDescending(s => s.Phone);
                    break;
                case "Email":
                    Member = Member.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    Member = Member.OrderByDescending(s => s.Email);
                    break;
                default:  // Name ascending 
                    Member = Member.OrderBy(s => s.UserName);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            ViewBag.ListMember = Member.ToList().ToPagedList(pageNumber, pageSize);
            return View();
        }
    }
}