﻿using System;
using System.Linq;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class OwnerDAO
    {
        KoiManagementEntities db = null;
        public OwnerDAO()
        {
            db = new KoiManagementEntities();
        }

        public bool AddOwner(Owner owner)
        {
            try
            {
                db.Owners.Add(owner);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public String GetOwnerName(int KoiId)
        {
            String OwnerName = String.Empty;
            var Owner = db.Owners.Where(p => p.KoiID == KoiId && p.Status).Select(p => p.Member.Name).ToList();
            if (Owner != null && Owner.Count > 0)
            {
                OwnerName = Owner.ElementAt(0);
            }
            return OwnerName;
        }

        public String GetOwnerID(int KoiId)
        {
            String OwnerName = String.Empty;
            var Owner = db.Owners.Where(p => p.KoiID == KoiId && p.Status).Select(p => p.Member. MemberID).ToList();
            if (Owner != null && Owner.Count > 0)
            {
                OwnerName = Owner.ElementAt(0).ToString();
            }
            return OwnerName;
        }
    }
}
