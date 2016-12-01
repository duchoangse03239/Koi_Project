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
            return db.KoiFarms.Where(f => f.Status==1).ToList();
        }

        public List<KoiFarm> GetListKoiFarmByMemberId(int id)
        {
            return db.KoiFarms.Where(f => f.MemberID == id && f.Status==1).ToList();
        }

        public List<Koi> GetListKoiByKoiFarmId(int koifarmId)
        {
            var listKoi = db.Kois.Where(cu => cu.Owners.Any(c => c.KoiFarmID == koifarmId)).ToList();
            return listKoi;
        }

        public KoiFarm GetKoiFarmById(int koifarmId)
        {
            return db.KoiFarms.Find(koifarmId);
        }

        public KoiFarm GetKoiFarmDetail(int id)
        {
            return db.KoiFarms.Find(id);
        }

        public int CountKoiFarmbyOwnerId(int id)
        {
            var koifarm = db.KoiFarms.Count(p => p.MemberID == id && p.Status==1);
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

        public IQueryable<KoiFarm> KoiFarmFilter(KoiFarmFilterModel searchModel)
        {
            var koiFarm = db.KoiFarms.AsQueryable();
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.farmname))
                {
                    koiFarm = koiFarm.Where(p => p.FarmName.Contains(searchModel.farmname));
                }
                if (!string.IsNullOrEmpty(searchModel.address))
                {
                    koiFarm = koiFarm.Where(p => p.Address.Contains(searchModel.address));
                }
                if (!string.IsNullOrEmpty(searchModel.owner))
                {
                    koiFarm = koiFarm.Where(p => p.Member.Name.Contains(searchModel.owner));
                }
                if (!string.IsNullOrEmpty(searchModel.orderby))
                {
                    switch (searchModel.orderby)
                    {
                        case "newest":
                            //koiFarm =koiFarm.OrderBy(p => p.InfoDetails.OrderBy(v => v.Date).Select(v => v.DetailID).FirstOrDefault());
                            break;
                        case "review":
                          //  koiFarm = koiFarm.Include(p => p.Comments).OrderByDescending(p => p.Comments.Count);
                            break;
                        case "date_desc":
                            break;
                        default: // Name ascending 
                            break;
                    }
                }
                // kiểm tra tồn tại koi
                koiFarm = koiFarm.Where(p => p.Status==1);
            }
            else
            {
                return null;
            }
            return koiFarm;
        }
    }
}