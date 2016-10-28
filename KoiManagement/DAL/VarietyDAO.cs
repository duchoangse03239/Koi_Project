using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoiManagement.Models;

namespace Model.DAO
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
