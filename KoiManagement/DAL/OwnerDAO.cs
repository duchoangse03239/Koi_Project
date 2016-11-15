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

        public String GetOwnerID(int KoiId)
        {
            String OwnerName = String.Empty;
            var Owner = db.Owners.Where(p => p.KoiID == KoiId && p.Status).Select(p => p.Member. MemberID).ToList();
            if (Owner != null && Owner.Count > 0)
            {
                OwnerName = Owner.ElementAt(0).ToString();
            }
            return OwnerName;
        }

        public bool ChangeOwner(string username,int koiID)
        {
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    var memberid =db.Members.Where(k => k.UserName == username).Select(p => p.MemberID).FirstOrDefault();
                //   update status
                var updateOwner = db.Owners.FirstOrDefault(p => p.KoiID == koiID&&p.Status);
                updateOwner.DateOwerTo = DateTime.Now;
                updateOwner.Status = false;

                db.Owners.Attach(updateOwner);
                var entry = db.Entry(updateOwner);
                entry.State = EntityState.Modified;
                    entry.Property(e => e.Status).IsModified = true;
                    entry.Property(e => e.DateOwerTo).IsModified = true;
                    db.SaveChanges();
                    // Set column not change
                    //add new owner

                   // var InsertOwner = db.Owners.FirstOrDefault(p => p.KoiID == koiID && p.Status);
                    Owner InsertOwner = new Owner(memberid, koiID,null,DateTime.Now, null,true);
                    db.Owners.Add(InsertOwner);
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
