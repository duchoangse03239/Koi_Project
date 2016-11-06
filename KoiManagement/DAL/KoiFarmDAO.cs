using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class KoiFarmDAO
    {
        KoiManagementEntities db = null;
        public KoiFarmDAO()
        {
            db = new KoiManagementEntities();
        }
        public List<KoiFarm> GetListKoiFarm()
        {
            return db.KoiFarms.Where(f => f.Status).ToList();
        }

        public List<KoiFarm> GetListKoiFarmByMemberId(int id)
        {
            return db.KoiFarms.Where(f =>f.MemberID==id&& f.Status).ToList();
        }

        public List<Koi> GetListKoiByKoiFarmId(int koifarmId)
        {
            var listKoi = db.Kois.Where(cu => cu.Owners.Any(c => c.KoiFarmID == koifarmId)).ToList();
            return listKoi;
        }

        public KoiFarm GetKoiFarmDetail(int id)
        {
            return db.KoiFarms.Find(id);
        }

        public int CountKoiFarmbyOwnerId(int id)
        {
            var koifarm = db.KoiFarms.Count(p => p.MemberID == id && p.Status);
            return koifarm;
        }
        public bool AddKoiFarm(KoiFarm koiFarm)
        {
            try
            {
                db.KoiFarms.Add(koiFarm);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public int EditKoiFarm(KoiFarm koiFarm)
        {
            try
            {
                db.KoiFarms.Attach(koiFarm);
                var entry = db.Entry(koiFarm);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.Status).IsModified = false;
                entry.Property(e => e.MemberID).IsModified = false;
                return db.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }
    }
}