using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using KoiManagement.Common;
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

          public IQueryable<Koi> KoiFilterByKoiFarm(KoiFilterModel searchModel, int koifarmId)
        {
            var koi = db.Kois.AsQueryable();
            koi= db.Kois.Where(cu => cu.Owners.Any(c => c.KoiFarmID == koifarmId));
            if (searchModel != null)
            {
                if (!string.IsNullOrEmpty(searchModel.KoiName))
                {
                    string koiname = CommonFunction.convertToNormalString(searchModel.KoiName);
                    koi = koi.Where(p => p.KoiName.Contains(searchModel.KoiName));
                }
                if (!string.IsNullOrEmpty(searchModel.Username))
                {
                    koi = koi.Where(p => p.Owners.Where(o => o.Status).FirstOrDefault().Member.UserName.Equals(searchModel.Username));
                }
                if (!string.IsNullOrEmpty(searchModel.VarietyId))
                {
                    int? variety = CommonFunction.ToNullableInt(searchModel.VarietyId);
                    koi = koi.Where(p => p.VarietyID == variety);
                }
                if (!string.IsNullOrEmpty(searchModel.SizeFrom))
                {
                    decimal? size = CommonFunction.ToNullableDecimal(searchModel.SizeFrom);
                    koi = koi.Where(p => p.InfoDetails.OrderBy(u => u.Date).FirstOrDefault().Size >= size);
                }
                if (!string.IsNullOrWhiteSpace(searchModel.SizeTo))
                {
                    decimal? size = CommonFunction.ToNullableDecimal(searchModel.SizeTo);
                    koi = koi.Where(p => p.InfoDetails.OrderBy(u => u.Date).FirstOrDefault().Size <= size);
                }
                if (!string.IsNullOrEmpty(searchModel.Gender))
                {
                    koi = koi.Where(p => p.Gender == searchModel.Gender);
                }
                if (!string.IsNullOrEmpty(searchModel.Owner))
                {
                    //@@
                    // koi = koi.Where(p => p.KoiName.Contains(searchModel.KoiName));
                    koi = koi.Where(p => p.Owners.Where(o => o.Status).FirstOrDefault().Member.Name.Contains(searchModel.Owner));
                }
                if (!string.IsNullOrEmpty(searchModel.AgeFrom))
                {
                    int? age = CommonFunction.ToNullableInt(searchModel.AgeFrom);
                    if (age.HasValue)
                    {
                        DateTime? Month = CommonFunction.getDateFromMonth(age.Value);
                        if (Month != null)
                        {
                            koi = koi.Where(p => p.DoB >= Month);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchModel.AgeTo))
                {
                    int? age = CommonFunction.ToNullableInt(searchModel.AgeTo);
                    if (age.HasValue)
                    {
                        DateTime? Month = CommonFunction.getDateFromMonth(age.Value);
                        if (Month != null)
                        {
                            koi = koi.Where(p => p.DoB <= Month);
                        }
                    }
                }
                if (!string.IsNullOrEmpty(searchModel.orderby))
                {
                    switch (searchModel.orderby)
                    {
                        case "newest":
                            koi = koi.OrderBy(p => p.InfoDetails.OrderBy(v => v.Date).Select(v => v.DetailID).FirstOrDefault());
                            break;
                        case "review":
                            koi = koi.Include(p => p.Comments).OrderByDescending(p => p.Comments.Count);
                            break;
                        case "date_desc":
                            break;
                        default:  // Name ascending 
                            break;
                    }
                }
                // kiểm tra tồn tại koi
                koi = koi.Where(p => p.Status == 1);
            }
            else { return null; }
            return koi;
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