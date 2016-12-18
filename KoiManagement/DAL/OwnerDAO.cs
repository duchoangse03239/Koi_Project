using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class OwnerDAO
    {
        KoiManagementEntities db = null;
        public OwnerDAO()
        {
            db = new KoiManagementEntities();
        }

        public bool AddOwner(Owner owner)
        {
            try
            {
                db.Owners.Add(owner);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public Owner GetOwner(int KoiId)
        {
            var Owner = db.Owners.FirstOrDefault(p => p.KoiID == KoiId && p.Status);
            return Owner;
        }

        public Owner GetCurrentOwner(int DetailID, DateTime date)
        {
            var t = db.Owners.AsQueryable();
           // var Owner = db.Owners.Where(p => p.Koi.InfoDetails.Any(a=>a.DetailID == DetailID&&p.DateOwerFrom< date&& p.DateOwerTo.HasValue ? date < p.DateOwerTo : true ) ).ToList();
           // var m = Owner.FirstOrDefault();
            t = t.Where(p => p.Koi.InfoDetails.Any(a => a.DetailID == DetailID));
            t = t.Where(p => p.DateOwerFrom < date && p.DateOwerTo.HasValue ? date < p.DateOwerTo : true);
            var b = t.ToList();


            var Owner = db.Owners.Where(p => p.Koi.InfoDetails.Any(a => a.DetailID == DetailID));
            foreach (var item in Owner)
            {
                if (item.DateOwerTo == null)
                {
                    return item;
                }
                else if(item.DateOwerFrom< date&&item.DateOwerTo>date)
                {
                    return item;
                }
            }
            if (b.Count > 0)
            {
                return b.FirstOrDefault();
            }
             return null;
        }


        public bool ChangeOwner(int notiID ,int memberid, int koiID)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //  update status cho chủ sở hữu cũ
                    var updateOwner = db.Owners.FirstOrDefault(p => p.KoiID == koiID&&p.Status);
                    updateOwner.DateOwerTo = DateTime.Now;
                    updateOwner.KoiFarmID = null;
                    updateOwner.Status = false;

                    db.Owners.Attach(updateOwner);
                    var entry = db.Entry(updateOwner);
                    entry.State = EntityState.Modified;
                    entry.Property(e => e.Status).IsModified = true;
                    entry.Property(e => e.KoiFarmID).IsModified = true;
                    entry.Property(e => e.DateOwerTo).IsModified = true;
                    db.SaveChanges();
                   
                    //add new owner
                    Owner InsertOwner = new Owner(memberid, koiID,null,DateTime.Now, null,true);
                    db.Owners.Add(InsertOwner);
                    db.SaveChanges();

                    //update trạng thái thông báo
                    var Notifi = db.Notifications.Find(notiID);
                    Notifi.isRead = true;
                    db.Notifications.Attach(Notifi);
                    var entry1 = db.Entry(Notifi);
                    entry1.State = EntityState.Modified;
                    entry1.Property(e => e.isRead).IsModified = true;
                    db.SaveChanges();
                    
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                    //throw; //Not in MSDN article, but recommended so the exception still bubbles up
                    return false;
                }
            }
        }

        public int getKoiFarmbyMember(int memberid)
        {
           var koifarmid= db.KoiFarms.Where(p => p.MemberID == memberid).Select(p => p.KoifarmID);
            if (koifarmid.Any())
            {
                return koifarmid.First();
            }
            return 0;
        }

        public List<Owner> GetAllOwnersByKoiID(int KoiID)
        {
            return db.Owners.Where(p => p.KoiID == KoiID).ToList();
        }

        public bool AddListKoiToKoiFarm(int[] listKoi, int koifarm)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    for(int i=0; i< listKoi.Length; i++)
                    {
                        var koiid = listKoi[i];
                        var Owner =db.Owners.Where(p =>  p.KoiID == koiid && p.Status).FirstOrDefault();
                        if (Owner != null)
                        {
                            Owner.KoiFarmID = koifarm;
                        db.Owners.Attach(Owner);
                        db.Entry(Owner).Property(x => x.KoiFarmID).IsModified = true;
                         db.SaveChanges();
                        }
                    }
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                    //throw; //Not in MSDN article, but recommended so the exception still bubbles up
                    return false;
                }
            }
        }
    }
}
