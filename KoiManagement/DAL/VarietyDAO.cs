using System.Collections.Generic;
using System.Linq;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class VarietyDAO
    {
        KoiManagementEntities db = null;
        public VarietyDAO()
        {
            db = new KoiManagementEntities();
        }

        public List<Variety> getListVariety()
        {
            return db.Varieties.ToList();
        }

        public List<Variety> getListMainVariety()
        {
           return  db.Varieties.Where(p => p.VarietyDetailID == null).ToList();
        }

    }
}
