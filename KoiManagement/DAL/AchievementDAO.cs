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

        /// <summary>
        /// Lấy id lớn nhất +1 của achievement
        /// </summary>
        /// <returns></returns>
        public int GetMaxAchiID()
        {
            var count = db.Achievements.Count();
                
                if (count==0)
            {
                return count = 1;
            }
            return db.Achievements.Max(g => g.AchievementID)+1; ;
        }


        public bool AddAchievement(Achievement ac)
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

        public int EditAchievement(Achievement ac)
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
        public List<Achievement> GetListAchievements(int id)
        {
            return db.Achievements.Where(p => p.KoiID == id && p.Status).ToList();
        }
    }
}