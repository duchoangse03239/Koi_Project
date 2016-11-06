using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class AchievementDAO
    {
        KoiManagementEntities db = null;
        public AchievementDAO()
        {
            db = new KoiManagementEntities();
        }

        public bool AddAchievemen(Achievement ac)
        {
            try
            {
                db.Achievements.Add(ac);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int EditAchievemen(Achievement ac)
        {
            try
            {
                db.Achievements.Attach(ac);
                var entry = db.Entry(ac);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.Status).IsModified = false;


                return db.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }
    }
}