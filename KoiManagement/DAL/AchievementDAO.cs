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

        public bool AddAchievemen(Achivement ac)
        {
            try
            {
                db.Achivements.Add(ac);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int EditAchievemen(Achivement ac)
        {
            try
            {
                db.Achivements.Attach(ac);
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