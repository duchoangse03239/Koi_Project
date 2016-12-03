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

        public int getVarityIdByName(string varietyName)
        {
            return db.Varieties.Where(p => p.VarietyName == varietyName).Select(p => p.VarietyID).FirstOrDefault();
        }

        public List<Variety> getListMainVariety()
        {
           return  db.Varieties.Where(p=>p.VarietyName!="Chưa rõ").ToList();
        }

    }
}
