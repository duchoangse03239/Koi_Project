using System;
using System.Collections.Generic;
using System.Globalization;
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
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
            var Member  = db.Members.AsQueryable();
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
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
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
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
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

        public IQueryable<Report> MemberReport(string searchString, string sortOrder)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
            var MemberReport = db.Reports.AsQueryable();
            //loc type
            DateTime Date ;
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM-dd-yyyy", "MM/dd/yyyy" };
            DateTime.TryParseExact(searchString, formats, CultureInfo.InvariantCulture,DateTimeStyles.None,out Date);
            MemberReport = MemberReport.Where(p => p.ObjectType == "Member");
            var listMember = db.Members.Where(p => p.UserName.Contains(searchString)).Select(p => p.MemberID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
               // MemberReport = MemberReport.Where(s => s.DateTime.ToString().Contains(searchString) || s.any.Contains(searchString) || s.Member.Phone.Contains(searchString) || s.Member.Name.Contains(searchString)));

                MemberReport = MemberReport.Where(s =>  listMember.Any(p=>p==s.ObjectId)|| listMember.Any(p => p == s.MemberID)|| s.DateTime== Date);
            }
            //sort
            switch (sortOrder)
            {
                case "Date_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.DateTime);
                    break;
                case "UserName":
                    MemberReport = MemberReport.OrderBy(s => s.MemberID);
                    break;
                case "UserName_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.MemberID);
                    break;
                case "UserName1":
                    MemberReport = MemberReport.OrderBy(s => s.ObjectId);
                    break;
                case "UserName1_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.ObjectId);
                    break;
                case "Status":
                    MemberReport = MemberReport.Where(s => s.Status);
                    break;
                case "AllStatus":
                    MemberReport = MemberReport.Where(s => !s.Status);
                    break;
                default:  // Name ascending 
                    MemberReport = MemberReport.OrderBy(s => s.DateTime);
                    break;
            }
            return MemberReport;
        }

        public IQueryable<Report> KoiReport(string searchString, string sortOrder)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
            var KoiReport = db.Reports.AsQueryable();
            //loc type
            DateTime Date;
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM-dd-yyyy", "MM/dd/yyyy" };
            DateTime.TryParseExact(searchString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out Date);
            KoiReport = KoiReport.Where(p => p.ObjectType == "Koi");
            var listMember = db.Owners.Where(p => p.Member.Name.Contains(searchString) && p.Status).Select(p=>p.MemberID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                // KoiReport = KoiReport.Where(s => s.DateTime.ToString().Contains(searchString) || s.any.Contains(searchString) || s.Member.Phone.Contains(searchString) || s.Member.Name.Contains(searchString)));

                KoiReport = KoiReport.Where(s => listMember.Any(p => p == s.ObjectId) || listMember.Any(p => p == s.MemberID) || s.DateTime == Date);
            }
            //sort
            switch (sortOrder)
            {
                case "Date_desc":
                    KoiReport = KoiReport.OrderByDescending(s => s.DateTime);
                    break;
                case "UserName":
                    KoiReport = KoiReport.OrderBy(s => s.MemberID);
                    break;
                case "UserName_desc":
                    KoiReport = KoiReport.OrderByDescending(s => s.MemberID);
                    break;
                case "UserName1":
                    KoiReport = KoiReport.OrderBy(s => s.ObjectId);
                    break;
                case "UserName1_desc":
                    KoiReport = KoiReport.OrderByDescending(s => s.ObjectId);
                    break;
                case "Status":
                    KoiReport = KoiReport.Where(s => s.Status);
                    break;
                case "AllStatus":
                    KoiReport = KoiReport.Where(s => !s.Status);
                    break;
                default:  // Name ascending 
                    KoiReport = KoiReport.OrderBy(s => s.DateTime);
                    break;
            }

            return KoiReport;
        }

        public IQueryable<Report> KoiFarmReport(string searchString, string sortOrder)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
            var KoiFarmReport = db.Reports.AsQueryable();
            //loc type
            DateTime Date;
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM-dd-yyyy", "MM/dd/yyyy" };
            DateTime.TryParseExact(searchString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out Date);
            KoiFarmReport = KoiFarmReport.Where(p => p.ObjectType == "KoiFarm");
            var listMember = db.Members.Where(p => p.UserName.Contains(searchString)).Select(p => p.MemberID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                // KoiFarmReport = KoiFarmReport.Where(s => s.DateTime.ToString().Contains(searchString) || s.any.Contains(searchString) || s.Member.Phone.Contains(searchString) || s.Member.Name.Contains(searchString)));

                KoiFarmReport = KoiFarmReport.Where(s => listMember.Any(p => p == s.ObjectId) || listMember.Any(p => p == s.MemberID) || s.DateTime == Date);
            }
            //sort
            switch (sortOrder)
            {
                case "Date_desc":
                    KoiFarmReport = KoiFarmReport.OrderByDescending(s => s.DateTime);
                    break;
                case "UserName":
                    KoiFarmReport = KoiFarmReport.OrderBy(s => s.MemberID);
                    break;
                case "UserName_desc":
                    KoiFarmReport = KoiFarmReport.OrderByDescending(s => s.MemberID);
                    break;
                case "KoiFarmID":
                    KoiFarmReport = KoiFarmReport.OrderBy(s => s.ObjectId);
                    break;
                case "KoiFarmID_desc":
                    KoiFarmReport = KoiFarmReport.OrderByDescending(s => s.ObjectId);
                    break;
                case "Status":
                    KoiFarmReport = KoiFarmReport.Where(s => s.Status);
                    break;
                case "AllStatus":
                    KoiFarmReport = KoiFarmReport.Where(s => !s.Status);
                    break;
                default:  // Name ascending 
                    KoiFarmReport = KoiFarmReport.OrderBy(s => s.DateTime);
                    break;
            }
            return KoiFarmReport;
        }
        public IQueryable<Report> QuesTionReport(string searchString, string sortOrder)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
            var MemberReport = db.Reports.AsQueryable();
            //loc type
            DateTime Date;
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM-dd-yyyy", "MM/dd/yyyy" };
            DateTime.TryParseExact(searchString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out Date);
            MemberReport = MemberReport.Where(p => p.ObjectType == "Question");
            var listMember = db.Members.Where(p => p.UserName.Contains(searchString)).Select(p => p.MemberID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                // MemberReport = MemberReport.Where(s => s.DateTime.ToString().Contains(searchString) || s.any.Contains(searchString) || s.Member.Phone.Contains(searchString) || s.Member.Name.Contains(searchString)));

                MemberReport = MemberReport.Where(s => listMember.Any(p => p == s.ObjectId) || listMember.Any(p => p == s.MemberID) || s.DateTime == Date);
            }
            //sort
            switch (sortOrder)
            {
                case "Date_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.DateTime);
                    break;
                case "UserName":
                    MemberReport = MemberReport.OrderBy(s => s.MemberID);
                    break;
                case "UserName_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.MemberID);
                    break;
                case "UserName1":
                    MemberReport = MemberReport.OrderBy(s => s.ObjectId);
                    break;
                case "UserName1_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.ObjectId);
                    break;
                case "Status":
                    MemberReport = MemberReport.Where(s => s.Status);
                    break;
                case "AllStatus":
                    MemberReport = MemberReport.Where(s => !s.Status);
                    break;
                default:  // Name ascending 
                    MemberReport = MemberReport.OrderBy(s => s.DateTime);
                    break;
            }
            return MemberReport;
        }
        public IQueryable<Report> AnswerReport(string searchString, string sortOrder)
        {
            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.Trim();
            }
            var MemberReport = db.Reports.AsQueryable();
            //loc type
            DateTime Date;
            string[] formats = { "yyyy-MM-dd", "yyyy/MM/dd", "dd-MM-yyyy", "dd/MM/yyyy", "MM-dd-yyyy", "MM/dd/yyyy" };
            DateTime.TryParseExact(searchString, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out Date);
            MemberReport = MemberReport.Where(p => p.ObjectType == "Answer");
            var listMember = db.Members.Where(p => p.UserName.Contains(searchString)).Select(p => p.MemberID).ToList();
            if (!String.IsNullOrEmpty(searchString))
            {
                // MemberReport = MemberReport.Where(s => s.DateTime.ToString().Contains(searchString) || s.any.Contains(searchString) || s.Member.Phone.Contains(searchString) || s.Member.Name.Contains(searchString)));

                MemberReport = MemberReport.Where(s => listMember.Any(p => p == s.ObjectId) || listMember.Any(p => p == s.MemberID) || s.DateTime == Date);
            }
            //sort
            switch (sortOrder)
            {
                case "Date_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.DateTime);
                    break;
                case "UserName":
                    MemberReport = MemberReport.OrderBy(s => s.MemberID);
                    break;
                case "UserName_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.MemberID);
                    break;
                case "UserName1":
                    MemberReport = MemberReport.OrderBy(s => s.ObjectId);
                    break;
                case "UserName1_desc":
                    MemberReport = MemberReport.OrderByDescending(s => s.ObjectId);
                    break;
                case "Status":
                    MemberReport = MemberReport.Where(s => s.Status);
                    break;
                case "AllStatus":
                    MemberReport = MemberReport.Where(s => !s.Status);
                    break;
                default:  // Name ascending 
                    MemberReport = MemberReport.OrderBy(s => s.DateTime);
                    break;
            }
            return MemberReport;
        }
    }
}
