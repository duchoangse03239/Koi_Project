using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoiManagement.Models;

namespace Model.DAO
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
    }
}
