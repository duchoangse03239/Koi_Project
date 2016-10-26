using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoiManagement.Models;

namespace Model.DAO
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

        public bool AddTranKoi(Koi koi,int memberID, string image,decimal size)
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

    }
}
