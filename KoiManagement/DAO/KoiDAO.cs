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

    }
}
