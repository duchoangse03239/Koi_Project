using System;
using System.Collections.Generic;
using System.Linq;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class InfoDetailDAO
    {
        KoiManagementEntities db = null;
        public InfoDetailDAO()
        {
            db = new KoiManagementEntities();
        }

        public bool AddInfoDetail(InfoDetail info, List<Medium> media)
        {
            // Transaction
            using (var dbContextTransaction = db.Database.BeginTransaction())
            {
                try
                {
                    db.InfoDetails.Add(info);
                    db.SaveChanges();
                    var DetailID = info.DetailID;
                    foreach (var item in media)
                    {
                        item.ModelId = DetailID;
                        db.Media.Add(item);
                        db.SaveChanges();
                    }
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback(); //Required according to MSDN article 
                    //throw; //Not in MSDN article, but recommended so the exception still bubbles up
                     return false;
                }
                return true;
            }
        }

        public List<InfoDetail> GetlistInfoDetails(int koiId)
        {
            return db.InfoDetails.Where(p => p.KoiID == koiId).ToList();
        }
        public int GetMaxDetailID()
        {
            return db.InfoDetails.Max(g => g.DetailID) + 1;
        }
       

    }
}
