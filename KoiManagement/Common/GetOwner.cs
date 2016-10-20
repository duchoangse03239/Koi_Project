using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model.Entities;

namespace KoiManagement.Common
{
    public class GetOwner
    {
        private static KoiManagementEntities db = new KoiManagementEntities();
        /// <summary>
        /// Lấy owner name theo KoiID
        /// </summary>
        /// <param name="KoiId">KoiId</param>
        /// <returns>OwnerName</returns>
        public static String GetOwnerName(int KoiId)
        {
            String OwnerName = String.Empty;
            var Owner = db.Owners.Where(p => p.KoiID == KoiId && p.Status == true).Select(p => p.Member.Name).ToList();
            if (Owner!=null && Owner.Count > 0)
            {
                OwnerName = Owner.ElementAt(0);
            }
            return OwnerName;
        }
    }
}