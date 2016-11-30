using System;
using System.Collections.Generic;
using System.Linq;
using KoiManagement.Models;

namespace KoiManagement.DAL
{
    public class AdminDAO
    {
        KoiManagementEntities db = null;
        public AdminDAO()
        {
            db = new KoiManagementEntities();
        }

        public IQueryable<Member> getlistMember(string searchString,string sortOrder)
        {
            var Member  = db.Members.AsQueryable(); ;
            if (!String.IsNullOrEmpty(searchString))
            {
                Member = Member.Where(s => s.UserName.Contains(searchString) || s.Name.Contains(searchString) || s.Phone.Contains(searchString) || s.Email.Contains(searchString));
            }
            //sort
            switch (sortOrder)
            {
                case "name_desc":
                    Member = Member.OrderByDescending(s => s.UserName);
                    break;
                case "Name":
                    Member = Member.OrderBy(s => s.Name);
                    break;
                case "Name_desc":
                    Member = Member.OrderByDescending(s => s.Name);
                    break;
                case "Phone":
                    Member = Member.OrderBy(s => s.Phone);
                    break;
                case "Phone_desc":
                    Member = Member.OrderByDescending(s => s.Phone);
                    break;
                case "Email":
                    Member = Member.OrderBy(s => s.Email);
                    break;
                case "Email_desc":
                    Member = Member.OrderByDescending(s => s.Email);
                    break;
                case "Status":
                    Member = Member.Where(s => s.Status);
                    break;
                case "AllStatus":
                    Member = Member.Where(s => !s.Status);
                    break;
                case "Role":
                    Member = Member.Where(s => s.Role == 1);
                    break;
                case "AllRole":
                    Member = Member.Where(s => s.Role != 1);
                    break;
                default:  // Name ascending 
                    Member = Member.OrderBy(s => s.UserName);
                    break;
            }
            return Member;
        }

        public IQueryable<Koi> getlistKoi(string searchString, string sortOrder)
        {
            var Koi = db.Kois.AsQueryable(); 
            if (!String.IsNullOrEmpty(searchString))
            {
                Koi = Koi.Where(s => s.KoiID.ToString()==searchString||s.KoiName.Contains(searchString) ||  s.Owners.FirstOrDefault(o => o.Status).Member.Name.Contains(searchString) );
               
            }
            //sort
            switch (sortOrder)
            {       
                case "koiID_desc":
                    Koi = Koi.OrderByDescending(s => s.KoiID);
                    break;
                case "Variety":
                    Koi = Koi.OrderBy(s => s.VarietyID);
                    break;
                case "Variety_desc":
                    Koi = Koi.OrderByDescending(s => s.VarietyID);
                    break;
                case "KoiName":
                    Koi = Koi.OrderBy(s => s.KoiName);
                    break;
                case "KoiName_desc":
                    Koi = Koi.OrderByDescending(s => s.KoiName);
                    break;
                case "Dob":
                    Koi = Koi.OrderBy(s => s.DoB);
                    break;
                case "Dob_desc":
                    Koi = Koi.OrderByDescending(s => s.DoB);
                    break;
                //case "Email":
                //    Koi = Koi.OrderBy(s => s.Email);
                //    break;
                //case "Email_desc":
                //    Koi = Koi.OrderByDescending(s => s.Email);
                //    break;
                case "Status":
                    Koi = Koi.Where(s => s.Status==1);
                    break;
                case "AllStatus":
                    Koi = Koi.Where(s => s.Status != 1);
                    break;
                default:  // Name ascending 
                    Koi = Koi.OrderBy(s => s.KoiID);
                    break;
            }
            return Koi;
        }
        public IQueryable<KoiFarm> getlistKoiFarm(string searchString, string sortOrder)
        {
            var KoiFarm = db.KoiFarms.AsQueryable(); 
            if (!String.IsNullOrEmpty(searchString))
            {
                KoiFarm = KoiFarm.Where(s => s.FarmName.Contains(searchString) || s.Address.Contains(searchString) || s.Member.Phone.Contains(searchString) || s.Member.Name.Contains(searchString));
            }
            //sort
            switch (sortOrder)
            {
                case "FarmName_desc":
                    KoiFarm = KoiFarm.OrderByDescending(s => s.FarmName);
                    break;
                case "Name":
                    KoiFarm = KoiFarm.OrderBy(s => s.Member.Name);
                    break;
                case "Name_desc":
                    KoiFarm = KoiFarm.OrderByDescending(s => s.Member.Name);
                    break;
                case "Phone":
                    KoiFarm = KoiFarm.OrderBy(s => s.Member.Phone);
                    break;
                case "Phone_desc":
                    KoiFarm = KoiFarm.OrderByDescending(s => s.Member.Phone);
                    break;
                case "Address":
                    KoiFarm = KoiFarm.OrderBy(s => s.Address);
                    break;
                case "Address_desc":
                    KoiFarm = KoiFarm.OrderByDescending(s => s.Address);
                    break;
                case "Status":
                    KoiFarm = KoiFarm.Where(s => s.Status==1);
                    break;
                case "AllStatus":
                    KoiFarm = KoiFarm.Where(s => s.Status != 1);
                    break;
                default:  // Name ascending 
                    KoiFarm = KoiFarm.OrderBy(s => s.FarmName);
                    break;
            }
            return KoiFarm;
        }

    }
}
