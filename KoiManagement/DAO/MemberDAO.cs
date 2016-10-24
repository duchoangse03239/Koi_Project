using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoiManagement.Models;

namespace Model.DAO
{
    public class MemberDAO
    {
        KoiManagementEntities db = null;
        public MemberDAO()
        {
            db = new KoiManagementEntities();
        }

        public Member GetMemberByNameAndPass(string username, string pass)
        {
             var mem = db.Members.Where(p => p.UserName.Equals(username) && p.Password.Equals(pass)).ToList();
            db.Database.Log = Console.Write;
            if (mem.Count == 0)
            {
                return null;
            }
            else return mem.First();

        }

        public bool CheckExistUserName(string username)
        {
            var member =db.Members.Where(p => p.UserName == username).ToList();
            return member.Count <= 0;
        }

        public bool CheckExistEmail(string email)
        {
            var member = db.Members.Where(p => p.Email == email).ToList();
            return member.Count <= 0;
        }

        public Member GetMemberByEmail(string email)
        {
            var dbmem = db.Members.Where(p => p.Email == email &&p.Status).ToList();
            if (dbmem.Count>0)
            {
                     return dbmem.ElementAt(0);
            }
            return null;
        }

        public bool AddMember(Member mem)
        {
            try { 
            db.Members.Add(mem);
            db.SaveChanges();
            return true;
            }
            catch
            {
                return false;
            }
        }

        public string GetOldPass(string memberId)
        {
            var member = db.Members.Find(int.Parse(memberId));
            return member.Password;
        }

        public int ChangePass(string memberId, string password)
        {
            var member = db.Members.Find(int.Parse(memberId));
            member.Password = password;
            db.Members.Attach(member);
            db.Entry(member).Property(x => x.Password).IsModified = true;
            return db.SaveChanges();
        }

        public int ChangePass1(string memberId, string password)
        {
            var member = db.Members.Find(int.Parse(memberId));
            member.Password = password;
            db.Members.Attach(member);
            db.Entry(member).Property(x => x.Password).IsModified = true;
            return db.SaveChanges();
        }
    }
}
