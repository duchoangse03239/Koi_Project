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

        public bool AddInfoDetail(InfoDetail info)
        {
            try
            {
                db.InfoDetails.Add(info);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public List<InfoDetail> GetlistInfoDetails(int koiId)
        {
            return db.InfoDetails.Where(p => p.KoiID == koiId).ToList();
        }

        //get detail
        //get achievemment

        //get image

    }
}
