using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using KoiManagement.Common;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class KoiDAO
    {
        KoiManagementEntities db = null;
        public KoiDAO()
        {
            db = new KoiManagementEntities();
        }

        public int GetMaxKoiID()
        {
            return  db.Kois.Max(g => g.KoiID) + 1;
        }

        public List<Koi> GetListKoiByMember(int memberID)
        {
             var Owner = db.Owners.Where(p => p.MemberID == memberID).ToList();
            if (Owner.Count > 0)
            {
                var ListKois = new List<Koi>();

                foreach (var item1 in Owner)
                {
                    var kois = db.Kois.Where(p => p.KoiID == item1.KoiID&&p.Status).ToList();

                    if (kois.Count > 0)
                    {
                        ListKois.Add(kois.ElementAt(0));
                    }
                }

           return ListKois;
            }
            return null;
        }

        public int CountKoibyOwnerId(int id)
        {
            var ow = db.Owners.Where(p=>p.MemberID== id&& p.Status);
            return ow.Include(p => p.Koi).Count(); 
        }

        public bool AddKoi(Koi koi)
        {
            try
            {
                db.Kois.Add(koi);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool AddKoi(Koi koi,int memberID, string image,decimal size)
        {
            // Transaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.Kois.Add(koi);
                    db.SaveChanges();
                    var koinewID = koi.KoiID;

                    db.Owners.Add(new Owner(memberID, koinewID, null, DateTime.Now, null, true));
                    db.SaveChanges();

                    db.InfoDetails.Add(new InfoDetail(koinewID, DateTime.Now, 0, size, String.Empty, String.Empty, image,true));
                    db.SaveChanges();

                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                    throw; //Not in MSDN article, but recommended so the exception still bubbles up
                }
            }
            return true;
        }

        public int EditKoi(Koi koi)
        {
            try
            {
                db.Kois.Attach(koi);
                var entry = db.Entry(koi);
                entry.State = EntityState.Modified;
                // Set column not change
                entry.Property(e => e.Status).IsModified = false;
                entry.Property(e => e.Privacy).IsModified = false;
                entry.Property(e => e.DeadReason).IsModified = false;
                entry.Property(e => e.IsDead).IsModified = false;

                return db.SaveChanges();
            }
            catch
            {
                return 0;
            }
        }

        public IQueryable<Koi> KoiFilter(KoiFilterModel searchModel)
        {
            var koi = db.Kois.AsQueryable();
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
                    koi = koi.Where(p => p.VarietyID== variety);
                }
                if (!string.IsNullOrEmpty(searchModel.SizeFrom))
                {
                    decimal? size = CommonFunction.ToNullableDecimal(searchModel.SizeFrom);
                    koi = koi.Where(p => p.InfoDetails.OrderByDescending(u => u.DetailID).FirstOrDefault().Size >= size);
                }
                if (!string.IsNullOrWhiteSpace(searchModel.SizeTo))
                {
                    decimal? size = CommonFunction.ToNullableDecimal(searchModel.SizeTo);
                    koi = koi.Where(p => p.InfoDetails.OrderByDescending(u => u.DetailID).FirstOrDefault().Size <= size);
                }
                if (!string.IsNullOrEmpty(searchModel.Gender))
                {
                    koi = koi.Where(p => p.Gender== searchModel.Gender);
                }
                if (!string.IsNullOrEmpty(searchModel.Owner))
                {
                    //@@
                   // koi = koi.Where(p => p.KoiName.Contains(searchModel.KoiName));
                    koi = koi.Where(p => p.Owners.Where(o => o.Status).FirstOrDefault().Member.Name.Contains(searchModel.Owner)) ;
                }
                if (!string.IsNullOrEmpty(searchModel.Age))
                {
                    //@@
                   // koi = koi.Where(p => p.KoiName.Contains(searchModel.KoiName));
                }
                if (!string.IsNullOrEmpty(searchModel.orderby))
                {
                    switch (searchModel.orderby)
                    {
                        case "newest":
                            koi = koi.OrderBy(p => p.InfoDetails.OrderBy(v=>v.Date).Select(v=>v.DetailID).FirstOrDefault());
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
                koi = koi.Where(p => p.Status);
            }
            else { return null;}
            return koi;
        }


    }
}
