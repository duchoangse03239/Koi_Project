using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class NotificationDAO
    {
        KoiManagementEntities db = null;
        public NotificationDAO()
        {
            db = new KoiManagementEntities();
        }
        public bool AddNotification(Notification No)
        {
            try
            {
                db.Notifications.Add(No);
                db.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public int UpdateNotification(Notification No)
        {
            try
            {
                db.Notifications.Attach(No);
                var entry = db.Entry(No);
                entry.State = EntityState.Modified;
                entry.Property(x => x.isRead).IsModified = true;
                return db.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }

        public List<Notification> GetListNotifications(int memberID)
        {
            return db.Notifications.Where(p => p.MemberID == memberID && p.status).ToList();
        }
    }
}