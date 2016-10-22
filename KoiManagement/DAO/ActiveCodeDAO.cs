using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
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

        public Member GetMemberIdByActCode(String code)
        {


                var  mem = db.ActiveCodes.Where(p => p.ActCode == code).ToList();
                if (mem.Count > 0)
                {
                    return mem.First().Member;
                }
                else
                {
                    return null;
                }



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
