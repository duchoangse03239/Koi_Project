using System;
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


        public bool ChangeOwner(int notiID ,int memberid, int koiID)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    //  update status cho chủ sở hữu cũ
                    var updateOwner = db.Owners.FirstOrDefault(p => p.KoiID == koiID&&p.Status);
                    updateOwner.DateOwerTo = DateTime.Now;
                    updateOwner.Status = false;

                    db.Owners.Attach(updateOwner);
                    var entry = db.Entry(updateOwner);
                    entry.State = EntityState.Modified;
                    entry.Property(e => e.Status).IsModified = true;
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
    }
}
