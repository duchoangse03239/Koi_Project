﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using KoiManagement.Common;
using KoiManagement.Models;

namespace KoiManagement.Controllers
{
    public class LayoutController : Controller
    {
        private KoiManagementEntities db = new KoiManagementEntities();
        // GET: Layout
        public ActionResult Index()
        {
            return View();
        }
        [ChildActionOnly]
        public ActionResult Header()
        {
            // var model = "duc";
            //return View();
            if (Session[SessionAccount.SessionUserId] != null)
            {
                int userid = int.Parse(Session[SessionAccount.SessionUserId].ToString());
                ViewBag.NotiCount = db.Notifications.Count(p => p.MemberID == userid&&p.status&&!p.isRead);
                if (ViewBag.NotiCount == null)
                {
                    ViewBag.NotiCount = 0;
                }
                ViewBag.MessageCount = db.Messages.Count(p => (p.MemberID == userid || p.SenderID == userid) && p.Status && !p.IsRead&&p.ReplyID==null);
                if (ViewBag.MessageCount == null)
                {
                    ViewBag.MessageCount = 0;
                }
            }
            return View("~/Views/Shared/_Header.cshtml");
        }
    }
}