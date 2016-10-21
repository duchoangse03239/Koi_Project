using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoiManagement.Models;

namespace Model.DAO
{
    public class ActiveCodeDAO
    {
         KoiManagementEntities db = null;
         public ActiveCodeDAO()
        {
            db = new KoiManagementEntities();
        }

        public bool checkExistCode(String code)
        {
            return db.ActiveCodes.Any(p => p.ActCode == code && p.Status);
        }

        public bool AddActiveCode(ActiveCode actCode)
        {
            try
            {
                db.ActiveCodes.Add(actCode);
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
